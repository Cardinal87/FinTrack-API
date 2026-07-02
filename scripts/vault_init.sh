#!/bin/sh    

set -e

if vault status -format=json 2>/dev/null | jq -e '.initialized == true' > /dev/null; then
  echo 'Vault already initialized.'
  if vault status -format=json | jq -e '.sealed == true' > /dev/null; then
    echo 'ERROR: Vault is sealed, but no keys are stored.'
    echo 'Please unseal manually using the keys from the first initialization.'
    exit 1
  else
    echo 'Vault is already unsealed and ready to use.'
  fi
else
    echo 'Initializing Vault...'
    init_json=$(vault operator init -format=json)
    echo "$init_json" | jq '.' 

    keys=$(echo "$init_json" | jq -r '.unseal_keys_b64[]')
    token=$(echo "$init_json" | jq -r '.root_token')

    echo 'Unsealing Vault...'
    for key in $keys; do
    vault operator unseal "$key"
    done


    echo "Root token: $token"
    echo 'Vault initialization and unseal completed successfully.'

    echo 'Creating signing key...'
    vault login $token
    vault secrets enable transit
    vault write transit/keys/fintrack-api-signing-key type=ed25519
    vault write transit/keys/fintrack-api-signing-key/config \
        auto_rotate_period=30d \

    echo 'Creating policy...'
    vault policy write fintrack-api-policy /configs/fintrack-api-policy.hcl


    echo 'Configuring approle authetication...'
    vault auth enable approle
    vault write auth/approle/role/fintrack-api-role \
      secret_id_ttl="720h" \
      token_policies="fintrack-api-policy" \
      token_ttl="1h" \
      max_token_ttl="10h" 

    role_id=$(vault read -format=json auth/approle/role/fintrack-api-role/role-id | jq -r '.data.role_id')
    secret_id=$(vault write -f -format=json auth/approle/role/fintrack-api-role/secret-id | jq -r '.data.secret_id')

    

    jq -n \
      --arg rid "$role_id" \
      --arg sid "$secret_id" \
      '{ "HashicorpVaultOptions": { "RoleID": $rid, "SecretID": $sid } }' > /shared/vault-creds.json
    
    chmod 644 /shared/vault-creds.json

    echo "API Role Id ${role_id}"
    echo "API Secret Id ${secret_id}"

fi