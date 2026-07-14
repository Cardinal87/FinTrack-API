#!/bin/sh    

set -e


if vault status -format=json 2>/dev/null | jq -e '.initialized == true' > /dev/null; then
  echo 'Vault already initialized.'
  if vault status -format=json | jq -e '.sealed == true' > /dev/null; then
    if [ ! -f /shared/keys.txt ]; then
        echo "Vault is sealed but no keys file found"
        exit 1
    fi 
    keys=$(cat /shared/keys.txt)

    echo 'Unsealing Vault...'
    for key in $keys; do
        vault operator unseal "$key"
    done
    
    echo "Vaule unsealed completely"

  else
    echo 'Vault is already unsealed and ready to use.'
  fi
else
    echo 'Initializing Vault...'
    init_json=$(vault operator init -format=json)
    echo "$init_json" | jq '.' 

    keys=$(echo "$init_json" | jq -r '.unseal_keys_b64[]')
    token=$(echo "$init_json" | jq -r '.root_token')

    echo $keys > /shared/keys.txt

    echo 'Unsealing Vault...'
    for key in $keys; do
    vault operator unseal "$key"
    done

    echo 'Vault initialization and unseal completed successfully.'

    echo 'Creating signing key...'
    vault login $token
    vault secrets enable transit
    vault write transit/keys/fintrack-api-signing-key type=ed25519
    vault write transit/keys/fintrack-api-signing-key/config \
        auto_rotate_period=30d \

    echo 'Creating policy...'
    vault policy write fintrack-api-policy /configs/fintrack-api-policy.hcl

    api_ip=$(getent hosts fintrack.api | awk '{ print $1 }')

    echo 'Configuring approle authetication...'
    vault auth enable approle
    vault write auth/approle/role/fintrack-api-role \
      bind_secret_id=false \
      token_bound_cidrs="10.0.0.30/32" \
      secret_id_bound_cidrs="10.0.0.30/32" \
      token_policies="fintrack-api-policy" \
      token_ttl="1h" \
      max_token_ttl="10h" 

    role_id=$(vault read -format=json auth/approle/role/fintrack-api-role/role-id | jq -r '.data.role_id')
    jq -n \
    --arg rid "$role_id" \
    '{ "HashicorpVaultOptions": { "RoleId": $rid } }' > /shared/roleid

    vault token revoke -self
    
    chmod 644 /shared/roleid

fi