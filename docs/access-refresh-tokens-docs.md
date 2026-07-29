# Architectural Plan: Access & Refresh Token Implementation

## 1. Overview
- **Access Token**: JWT, short-lived (15 minutes).
- **Refresh Token**: Opaque, long-lived (7 days), with mandatory rotation on every use.

## 2. Key Architectural Decisions

| Decision | Choice | Rationale |
| :--- | :--- | :--- |
| **Refresh Token Format** | Opaque (Base64Url), 32 bytes of cryptographically secure randomness | Prevents forgery or data extraction by the client or in case of a leak. |
| **Database Storage** | Only the SHA-256 hash of the token | Protects against DB compromise; attackers will not obtain the actual raw tokens. |
| **Cache Storage (Redis)** | Primary store for validation + PostgreSQL as fallback | Ensures fast-fail validation and offloads read traffic from the database. |
| **Revoke Handling** | **Tombstone Pattern (Negative Caching)** | Upon revocation, the Redis key is *not* deleted, but updated with `IsRevoked=true` and a TTL equal to the token's remaining lifetime. This prevents Cache Penetration (DDoS on the DB with old tokens). |
| **Rotation Strategy** | Simple Rotation | The old token is immediately invalidated upon a successful refresh request. |
| **Revoke Endpoint** | Requires a valid Access Token in the header | Prevents malicious revocation by an attacker who intercepted only the Refresh Token. |

---

## 3. Architecture Diagrams

### 3.1. Login / Token Issuance
```mermaid
sequenceDiagram
    participant Client
    participant API as API Controller
    participant MediatR as AuthUserHandler
    participant JwtSvc as JwtTokenService
    participant RefSvc as RefreshTokenService
    participant Redis as Redis Cache
    participant DB as PostgreSQL

    Client->>API: POST /api/auth/token {login, password}
    API->>MediatR: AuthUserCommand
    MediatR->>DB: Get user by login
    DB-->>MediatR: User entity
    MediatR->>JwtSvc: Generate access token
    JwtSvc-->>MediatR: Signed JWT
    MediatR->>RefSvc: Generate refresh token (userId)
    RefSvc->>RefSvc: Generate random bytes, hash (SHA-256)
    RefSvc->>DB: INSERT RefreshToken (Hash, UserId, ExpiresAt)
    RefSvc->>Redis: SET refresh_token:{hash} {UserId, ExpiresAt, IsRevoked=false} [TTL: 7d]
    RefSvc-->>MediatR: Raw opaque token
    MediatR-->>API: AuthResponse (Access + Refresh)
    API-->>Client: 200 OK {access_token, refresh_token, expires_in}
```


> **Note on 2FA Integration:** 
> If the user has Two-Factor Authentication (2FA) enabled, this endpoint behaves differently. 
> Instead of returning the final token pair, it validates credentials, generates a short-lived `challenge_token` (JWT), and returns `202 Accepted`. 
> The actual `access_token` and `refresh_token` described in this diagram are issued only after successful 2FA code verification via `POST /api/auth/2fa/complete`. 
> This diagram represents the final token issuance step (or the flow for users without 2FA).

### 3.2. Refresh (Rotation)
```mermaid
sequenceDiagram
    participant Client
    participant API as API Controller
    participant MediatR as RefreshTokenHandler
    participant RefSvc as RefreshTokenService
    participant Redis as Redis Cache
    participant DB as PostgreSQL
    participant JwtSvc as JwtTokenService

    Client->>API: POST /api/auth/token/refresh {refresh_token}
    API->>MediatR: RefreshTokenCommand
    MediatR->>RefSvc: RotateRefreshTokenAsync(rawToken)
    
    RefSvc->>RefSvc: Hash(rawToken) = oldHash
    RefSvc->>Redis: GET refresh_token:{oldHash}
    
    alt Cache Hit (Valid)
        Redis-->>RefSvc: Metadata
    else Cache Miss or Expired
        RefSvc->>DB: SELECT WHERE TokenHash = oldHash
        DB-->>RefSvc: RefreshToken entity
        RefSvc->>Redis: SET refresh_token:{oldHash} [TTL: remaining or 1h negative cache]
    end

    RefSvc->>RefSvc: Validate (Not Revoked, Not Expired)
    RefSvc->>RefSvc: Generate new random bytes, hash = newHash
    
    RefSvc->>DB: UPDATE old: IsRevoked=true, ReplacedByHash=newHash
    RefSvc->>DB: INSERT new: TokenHash=newHash, UserId, ExpiresAt
    RefSvc->>Redis: SET refresh_token:{oldHash} {IsRevoked=true} [TTL: remaining]
    RefSvc->>Redis: SET refresh_token:{newHash} {IsRevoked=false} [TTL: 7d]
    
    RefSvc-->>MediatR: RotationResult (UserId, newRawToken)
    MediatR->>DB: Get User by UserId (Safety check)
    MediatR->>JwtSvc: Generate new access token
    MediatR-->>API: AuthResponse
    API-->>Client: 200 OK {new access_token, new refresh_token, expires_in}
```

### 3.3. Revoke (Logout)
```mermaid
sequenceDiagram
    participant Client
    participant API as API Controller
    participant MediatR as RevokeTokenHandler
    participant RefSvc as RefreshTokenService
    participant Redis as Redis Cache
    participant DB as PostgreSQL

    Client->>API: POST /api/auth/token/revoke {refresh_token}
    Note over Client,API: Requires Authorization: Bearer <valid_access_token>
    API->>MediatR: RevokeTokenCommand
    MediatR->>RefSvc: RevokeRefreshTokenAsync(rawToken)
    
    RefSvc->>RefSvc: Hash(rawToken)
    RefSvc->>DB: SELECT WHERE TokenHash = hash
    alt Token exists and not revoked
        RefSvc->>DB: UPDATE IsRevoked=true, RevokedAt=NOW()
        RefSvc->>Redis: SET refresh_token:{hash} {IsRevoked=true} [TTL: remaining]
    end
    
    RefSvc-->>MediatR: Success
    MediatR-->>API: Success
    API-->>Client: 204 No Content
```