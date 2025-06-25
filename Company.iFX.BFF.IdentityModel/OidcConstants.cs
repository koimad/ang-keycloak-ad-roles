#pragma warning disable 1591

namespace Company.iFX.BFF.IdentityModel;

public static class OidcConstants
{
    public static class AuthorizeRequest
    {
        public const String Scope = "scope";
        public const String ResponseType = "response_type";
        public const String ClientId = "client_id";
        public const String RedirectUri = "redirect_uri";
        public const String State = "state";
        public const String ResponseMode = "response_mode";
        public const String Nonce = "nonce";
        public const String Display = "display";
        public const String Prompt = "prompt";
        public const String MaxAge = "max_age";
        public const String UiLocales = "ui_locales";
        public const String IdTokenHint = "id_token_hint";
        public const String LoginHint = "login_hint";
        public const String AcrValues = "acr_values";
        public const String CodeChallenge = "code_challenge";
        public const String CodeChallengeMethod = "code_challenge_method";
        public const String Request = "request";
        public const String RequestUri = "request_uri";
        public const String Resource = "resource";
        public const String DPoPKeyThumbprint = "dpop_jkt";
    }

    public static class AuthorizeErrors
    {
        // OAuth2 errors
        public const String InvalidRequest = "invalid_request";
        public const String UnauthorizedClient = "unauthorized_client";
        public const String AccessDenied = "access_denied";
        public const String UnsupportedResponseType = "unsupported_response_type";
        public const String InvalidScope = "invalid_scope";
        public const String ServerError = "server_error";
        public const String TemporarilyUnavailable = "temporarily_unavailable";
        public const String UnmetAuthenticationRequirements = "unmet_authentication_requirements";

        // OIDC errors
        public const String InteractionRequired = "interaction_required";
        public const String LoginRequired = "login_required";
        public const String AccountSelectionRequired = "account_selection_required";
        public const String ConsentRequired = "consent_required";
        public const String InvalidRequestUri = "invalid_request_uri";
        public const String InvalidRequestObject = "invalid_request_object";
        public const String RequestNotSupported = "request_not_supported";
        public const String RequestUriNotSupported = "request_uri_not_supported";
        public const String RegistrationNotSupported = "registration_not_supported";

        // resource indicator spec
        public const String InvalidTarget = "invalid_target";
    }

    public static class AuthorizeResponse
    {
        public const String Scope = "scope";
        public const String Code = "code";
        public const String AccessToken = "access_token";
        public const String ExpiresIn = "expires_in";
        public const String TokenType = "token_type";
        public const String RefreshToken = "refresh_token";
        public const String IdentityToken = "id_token";
        public const String State = "state";
        public const String SessionState = "session_state";
        public const String Issuer = "iss";
        public const String Error = "error";
        public const String ErrorDescription = "error_description";
    }

    public static class DeviceAuthorizationResponse
    {
        public const String DeviceCode = "device_code";
        public const String UserCode = "user_code";
        public const String VerificationUri = "verification_uri";
        public const String VerificationUriComplete = "verification_uri_complete";
        public const String ExpiresIn = "expires_in";
        public const String Interval = "interval";
    }

    public static class EndSessionRequest
    {
        public const String IdTokenHint = "id_token_hint";
        public const String PostLogoutRedirectUri = "post_logout_redirect_uri";
        public const String State = "state";
        public const String Sid = "sid";
        public const String Issuer = "iss";
        public const String UiLocales = "ui_locales";
    }

    public static class TokenRequest
    {
        public const String GrantType = "grant_type";
        public const String RedirectUri = "redirect_uri";
        public const String ClientId = "client_id";
        public const String ClientSecret = "client_secret";
        public const String ClientAssertion = "client_assertion";
        public const String ClientAssertionType = "client_assertion_type";
        public const String Assertion = "assertion";
        public const String Code = "code";
        public const String RefreshToken = "refresh_token";
        public const String Scope = "scope";
        public const String UserName = "username";
        public const String Password = "password";
        public const String CodeVerifier = "code_verifier";
        public const String TokenType = "token_type";
        public const String Algorithm = "alg";
        public const String Key = "key";
        public const String DeviceCode = "device_code";

        // token exchange
        public const String Resource = "resource";
        public const String Audience = "audience";
        public const String RequestedTokenType = "requested_token_type";
        public const String SubjectToken = "subject_token";
        public const String SubjectTokenType = "subject_token_type";
        public const String ActorToken = "actor_token";
        public const String ActorTokenType = "actor_token_type";
        
        // ciba
        public const String AuthenticationRequestId = "auth_req_id";
    }

    public static class BackchannelAuthenticationRequest
    {
        public const String Scope = "scope";
        public const String ClientNotificationToken = "client_notification_token";
        public const String AcrValues = "acr_values";
        public const String LoginHintToken = "login_hint_token";
        public const String IdTokenHint = "id_token_hint";
        public const String LoginHint = "login_hint";
        public const String BindingMessage = "binding_message";
        public const String UserCode = "user_code";
        public const String RequestedExpiry = "requested_expiry";
        public const String Request = "request";
        public const String Resource = "resource";
        public const String DPoPKeyThumbprint = "dpop_jkt";
    }

    public static class BackchannelAuthenticationRequestErrors
    {
        public const String InvalidRequestObject = "invalid_request_object";
        public const String InvalidRequest = "invalid_request";
        public const String InvalidScope = "invalid_scope";
        public const String ExpiredLoginHintToken = "expired_login_hint_token";
        public const String UnknownUserId = "unknown_user_id";
        public const String UnauthorizedClient = "unauthorized_client";
        public const String MissingUserCode = "missing_user_code";
        public const String InvalidUserCode = "invalid_user_code";
        public const String InvalidBindingMessage = "invalid_binding_message";
        public const String InvalidClient = "invalid_client";
        public const String AccessDenied = "access_denied";
        public const String InvalidTarget = "invalid_target";
    }

    public static class TokenRequestTypes
    {
        public const String Bearer = "bearer";
        public const String Pop = "pop";
    }

    public static class TokenErrors
    {
        public const String InvalidRequest = "invalid_request";
        public const String InvalidClient = "invalid_client";
        public const String InvalidGrant = "invalid_grant";
        public const String UnauthorizedClient = "unauthorized_client";
        public const String UnsupportedGrantType = "unsupported_grant_type";
        public const String UnsupportedResponseType = "unsupported_response_type";
        public const String InvalidScope = "invalid_scope";
        public const String AuthorizationPending = "authorization_pending";
        public const String AccessDenied = "access_denied";
        public const String SlowDown = "slow_down";
        public const String ExpiredToken = "expired_token";
        public const String InvalidTarget = "invalid_target";
        public const String InvalidDPoPProof = "invalid_dpop_proof";
        public const String UseDPoPNonce = "use_dpop_nonce";
    }

    public static class TokenResponse
    {
        public const String AccessToken = "access_token";
        public const String ExpiresIn = "expires_in";
        public const String TokenType = "token_type";
        public const String RefreshToken = "refresh_token";
        public const String IdentityToken = "id_token";
        public const String Error = "error";
        public const String ErrorDescription = "error_description";
        public const String BearerTokenType = "Bearer";
        public const String DPoPTokenType = "DPoP";
        public const String IssuedTokenType = "issued_token_type";
        public const String Scope = "scope";
    }

    public static class BackchannelAuthenticationResponse
    {
        public const String AuthenticationRequestId = "auth_req_id";
        public const String ExpiresIn = "expires_in";
        public const String Interval = "interval";
    }

    public static class PushedAuthorizationRequestResponse
    {
        public const String ExpiresIn = "expires_in";
        public const String RequestUri = "request_uri";
    }

    public static class TokenIntrospectionRequest
    {
        public const String Token = "token";
        public const String TokenTypeHint = "token_type_hint";
    }

    public static class RegistrationResponse
    {
        public const String Error = "error";
        public const String ErrorDescription = "error_description";
        public const String ClientId = "client_id";
        public const String ClientSecret = "client_secret";
        public const String RegistrationAccessToken = "registration_access_token";
        public const String RegistrationClientUri = "registration_client_uri";
        public const String ClientIdIssuedAt = "client_id_issued_at";
        public const String ClientSecretExpiresAt = "client_secret_expires_at";
        public const String SoftwareStatement = "software_statement";
    }

    public static class ClientMetadata
    {
        public const String RedirectUris = "redirect_uris";
        public const String ResponseTypes = "response_types";
        public const String GrantTypes = "grant_types";
        public const String ApplicationType = "application_type";
        public const String Contacts = "contacts";
        public const String ClientName = "client_name";
        public const String LogoUri = "logo_uri";
        public const String ClientUri = "client_uri";
        public const String PolicyUri = "policy_uri";
        public const String TosUri = "tos_uri";
        public const String JwksUri = "jwks_uri";
        public const String Jwks = "jwks";
        public const String SectorIdentifierUri = "sector_identifier_uri";
        public const String Scope = "scope";
        public const String PostLogoutRedirectUris = "post_logout_redirect_uris";
        public const String FrontChannelLogoutUri = "frontchannel_logout_uri";
        public const String FrontChannelLogoutSessionRequired = "frontchannel_logout_session_required";
        public const String BackchannelLogoutUri = "backchannel_logout_uri";
        public const String BackchannelLogoutSessionRequired = "backchannel_logout_session_required";
        public const String SoftwareId = "software_id";
        public const String SoftwareStatement = "software_statement";
        public const String SoftwareVersion = "software_version";
        public const String SubjectType = "subject_type";
        public const String TokenEndpointAuthenticationMethod = "token_endpoint_auth_method";
        public const String TokenEndpointAuthenticationSigningAlgorithm = "token_endpoint_auth_signing_alg";
        public const String DefaultMaxAge = "default_max_age";
        public const String RequireAuthenticationTime = "require_auth_time";
        public const String DefaultAcrValues = "default_acr_values";
        public const String InitiateLoginUri = "initiate_login_uri";
        public const String RequestUris = "request_uris";
        public const String IdentityTokenSignedResponseAlgorithm = "id_token_signed_response_alg";
        public const String IdentityTokenEncryptedResponseAlgorithm = "id_token_encrypted_response_alg";
        public const String IdentityTokenEncryptedResponseEncryption = "id_token_encrypted_response_enc";
        public const String UserinfoSignedResponseAlgorithm = "userinfo_signed_response_alg";
        public const String UserInfoEncryptedResponseAlgorithm = "userinfo_encrypted_response_alg";
        public const String UserinfoEncryptedResponseEncryption = "userinfo_encrypted_response_enc";
        public const String RequestObjectSigningAlgorithm = "request_object_signing_alg";
        public const String RequestObjectEncryptionAlgorithm = "request_object_encryption_alg";
        public const String RequestObjectEncryptionEncryption = "request_object_encryption_enc";
        public const String RequireSignedRequestObject = "require_signed_request_object";
        public const String AlwaysUseDPoPBoundAccessTokens = "dpop_bound_access_tokens";
    }

    public static class TokenTypes
    {
        public const String AccessToken = "access_token";
        public const String IdentityToken = "id_token";
        public const String RefreshToken = "refresh_token";
    }

    public static class TokenTypeIdentifiers
    {
        public const String AccessToken = "urn:ietf:params:oauth:token-type:access_token";
        public const String IdentityToken = "urn:ietf:params:oauth:token-type:id_token";
        public const String RefreshToken = "urn:ietf:params:oauth:token-type:refresh_token";
        public const String Saml11 = "urn:ietf:params:oauth:token-type:saml1";
        public const String Saml2 = "urn:ietf:params:oauth:token-type:saml2";
        public const String Jwt = "urn:ietf:params:oauth:token-type:jwt";
    }

    public static class AuthenticationSchemes
    {
        public const String AuthorizationHeaderBearer = "Bearer";
        public const String AuthorizationHeaderDPoP = "DPoP";
        
        public const String FormPostBearer = "access_token";
        public const String QueryStringBearer = "access_token";

        public const String AuthorizationHeaderPop = "PoP";
        public const String FormPostPop = "pop_access_token";
        public const String QueryStringPop = "pop_access_token";
    }

    public static class GrantTypes
    {
        public const String Password = "password";
        public const String AuthorizationCode = "authorization_code";
        public const String ClientCredentials = "client_credentials";
        public const String RefreshToken = "refresh_token";
        public const String Implicit = "implicit";
        public const String Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
        public const String JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        public const String DeviceCode = "urn:ietf:params:oauth:grant-type:device_code";
        public const String TokenExchange = "urn:ietf:params:oauth:grant-type:token-exchange";
        public const String Ciba = "urn:openid:params:grant-type:ciba";
    }

    public static class ClientAssertionTypes
    {
        public const String JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        public const String SamlBearer = "urn:ietf:params:oauth:client-assertion-type:saml2-bearer";
    }

    public static class ResponseTypes
    {
        public const String Code = "code";
        public const String Token = "token";
        public const String IdToken = "id_token";
        public const String IdTokenToken = "id_token token";
        public const String CodeIdToken = "code id_token";
        public const String CodeToken = "code token";
        public const String CodeIdTokenToken = "code id_token token";
    }

    public static class ResponseModes
    {
        public const String FormPost = "form_post";
        public const String Query = "query";
        public const String Fragment = "fragment";
    }

    public static class DisplayModes
    {
        public const String Page = "page";
        public const String Popup = "popup";
        public const String Touch = "touch";
        public const String Wap = "wap";
    }

    public static class PromptModes
    {
        public const String None = "none";
        public const String Login = "login";
        public const String Consent = "consent";
        public const String SelectAccount = "select_account";
        public const String Create = "create";
    }

    public static class CodeChallengeMethods
    {
        public const String Plain = "plain";
        public const String Sha256 = "S256";
    }

    public static class ProtectedResourceErrors
    {
        public const String InvalidToken = "invalid_token";
        public const String ExpiredToken = "expired_token";
        public const String InvalidRequest = "invalid_request";
        public const String InsufficientScope = "insufficient_scope";
    }

    public static class EndpointAuthenticationMethods
    {
        public const String PostBody = "client_secret_post";
        public const String BasicAuthentication = "client_secret_basic";
        public const String PrivateKeyJwt = "private_key_jwt";
        public const String TlsClientAuth = "tls_client_auth";
        public const String SelfSignedTlsClientAuth = "self_signed_tls_client_auth";
    }

    public static class AuthenticationMethods
    {
        public const String FacialRecognition = "face";
        public const String FingerprintBiometric = "fpt";
        public const String Geolocation = "geo";
        public const String ProofOfPossessionHardwareSecuredKey = "hwk";
        public const String IrisScanBiometric = "iris";
        public const String KnowledgeBasedAuthentication = "kba";
        public const String MultipleChannelAuthentication = "mca";
        public const String MultiFactorAuthentication = "mfa";
        public const String OneTimePassword = "otp";
        public const String PersonalIdentificationOrPattern = "pin";
        public const String ProofOfPossessionKey = "pop";
        public const String Password = "pwd";
        public const String RiskBasedAuthentication = "rba";
        public const String RetinaScanBiometric = "retina";
        public const String SmartCard = "sc";
        public const String ConfirmationBySms = "sms";
        public const String ProofOfPossessionSoftwareSecuredKey = "swk";
        public const String ConfirmationByTelephone = "tel";
        public const String UserPresenceTest = "user";
        public const String VoiceBiometric = "vbm";
        public const String WindowsIntegratedAuthentication = "wia";
    }

    public static class Algorithms
    {
        public const String None = "none";

        public static class Symmetric
        {
            public const String HS256 = "HS256";
            public const String HS384 = "HS384";
            public const String HS512 = "HS512";
        }

        public static class Asymmetric
        {
            public const String RS256 = "RS256";
            public const String RS384 = "RS384";
            public const String RS512 = "RS512";

            public const String ES256 = "ES256";
            public const String ES384 = "ES384";
            public const String ES512 = "ES512";

            public const String PS256 = "PS256";
            public const String PS384 = "PS384";
            public const String PS512 = "PS512";

        }
    }

    public static class Discovery
    {
        public const String Issuer = "issuer";

        // endpoints
        public const String AuthorizationEndpoint = "authorization_endpoint";
        public const String DeviceAuthorizationEndpoint = "device_authorization_endpoint";
        public const String TokenEndpoint = "token_endpoint";
        public const String UserInfoEndpoint = "userinfo_endpoint";
        public const String IntrospectionEndpoint = "introspection_endpoint";
        public const String RevocationEndpoint = "revocation_endpoint";
        public const String DiscoveryEndpoint = ".well-known/openid-configuration";
        public const String JwksUri = "jwks_uri";
        public const String EndSessionEndpoint = "end_session_endpoint";
        public const String CheckSessionIframe = "check_session_iframe";
        public const String RegistrationEndpoint = "registration_endpoint";
        public const String MtlsEndpointAliases = "mtls_endpoint_aliases";
        public const String PushedAuthorizationRequestEndpoint = "pushed_authorization_request_endpoint";

        // common capabilities
        public const String FrontChannelLogoutSupported = "frontchannel_logout_supported";
        public const String FrontChannelLogoutSessionSupported = "frontchannel_logout_session_supported";
        public const String BackChannelLogoutSupported = "backchannel_logout_supported";
        public const String BackChannelLogoutSessionSupported = "backchannel_logout_session_supported";
        public const String GrantTypesSupported = "grant_types_supported";
        public const String CodeChallengeMethodsSupported = "code_challenge_methods_supported";
        public const String ScopesSupported = "scopes_supported";
        public const String SubjectTypesSupported = "subject_types_supported";
        public const String ResponseModesSupported = "response_modes_supported";
        public const String ResponseTypesSupported = "response_types_supported";
        public const String ClaimsSupported = "claims_supported";
        public const String TokenEndpointAuthenticationMethodsSupported = "token_endpoint_auth_methods_supported";

        // more capabilities
        public const String ClaimsLocalesSupported = "claims_locales_supported";
        public const String ClaimsParameterSupported = "claims_parameter_supported";
        public const String ClaimTypesSupported = "claim_types_supported";
        public const String DisplayValuesSupported = "display_values_supported";
        public const String AcrValuesSupported = "acr_values_supported";
        public const String IdTokenEncryptionAlgorithmsSupported = "id_token_encryption_alg_values_supported";
        public const String IdTokenEncryptionEncValuesSupported = "id_token_encryption_enc_values_supported";
        public const String IdTokenSigningAlgorithmsSupported = "id_token_signing_alg_values_supported";
        public const String OpPolicyUri = "op_policy_uri";
        public const String OpTosUri = "op_tos_uri";
        public const String RequestObjectEncryptionAlgorithmsSupported = "request_object_encryption_alg_values_supported";
        public const String RequestObjectEncryptionEncValuesSupported = "request_object_encryption_enc_values_supported";
        public const String RequestObjectSigningAlgorithmsSupported = "request_object_signing_alg_values_supported";
        public const String RequestParameterSupported = "request_parameter_supported";
        public const String RequestUriParameterSupported = "request_uri_parameter_supported";
        public const String RequireRequestUriRegistration = "require_request_uri_registration";
        public const String ServiceDocumentation = "service_documentation";
        public const String TokenEndpointAuthSigningAlgorithmsSupported = "token_endpoint_auth_signing_alg_values_supported";
        public const String UILocalesSupported = "ui_locales_supported";
        public const String UserInfoEncryptionAlgorithmsSupported = "userinfo_encryption_alg_values_supported";
        public const String UserInfoEncryptionEncValuesSupported = "userinfo_encryption_enc_values_supported";
        public const String UserInfoSigningAlgorithmsSupported = "userinfo_signing_alg_values_supported";
        public const String TlsClientCertificateBoundAccessTokens = "tls_client_certificate_bound_access_tokens";
        public const String AuthorizationResponseIssParameterSupported = "authorization_response_iss_parameter_supported";
        public const String PromptValuesSupported = "prompt_values_supported";

        // CIBA
        public const String BackchannelTokenDeliveryModesSupported = "backchannel_token_delivery_modes_supported";
        public const String BackchannelAuthenticationEndpoint = "backchannel_authentication_endpoint";
        public const String BackchannelAuthenticationRequestSigningAlgValuesSupported = "backchannel_authentication_request_signing_alg_values_supported";
        public const String BackchannelUserCodeParameterSupported = "backchannel_user_code_parameter_supported";
        
        // DPoP
        public const String DPoPSigningAlgorithmsSupported = "dpop_signing_alg_values_supported";

        // PAR
        public const String RequirePushedAuthorizationRequests = "require_pushed_authorization_requests";
    }

    public static class BackchannelTokenDeliveryModes
    {
        public const String Poll = "poll";
        public const String Ping = "ping";
        public const String Push = "push";
    }

    public static class Events
    {
        public const String BackChannelLogout = "http://schemas.openid.net/event/backchannel-logout";
    }

    public static class BackChannelLogoutRequest
    {
        public const String LogoutToken = "logout_token";
    }

    public static class StandardScopes
    {
        /// <summary>REQUIRED. Informs the Authorization Server that the Client is making an OpenID Connect request. If the <c>openid</c> scope value is not present, the behavior is entirely unspecified.</summary>
        public const String OpenId = "openid";
        /// <summary>OPTIONAL. This scope value requests access to the End-User's default profile Claims, which are: <c>name</c>, <c>family_name</c>, <c>given_name</c>, <c>middle_name</c>, <c>nickname</c>, <c>preferred_username</c>, <c>profile</c>, <c>picture</c>, <c>website</c>, <c>gender</c>, <c>birthdate</c>, <c>zoneinfo</c>, <c>locale</c>, and <c>updated_at</c>.</summary>
        public const String Profile = "profile";
        /// <summary>OPTIONAL. This scope value requests access to the <c>email</c> and <c>email_verified</c> Claims.</summary>
        public const String Email = "email";
        /// <summary>OPTIONAL. This scope value requests access to the <c>address</c> Claim.</summary>
        public const String Address = "address";
        /// <summary>OPTIONAL. This scope value requests access to the <c>phone_number</c> and <c>phone_number_verified</c> Claims.</summary>
        public const String Phone = "phone";
        /// <summary>This scope value MUST NOT be used with the OpenID Connect Implicit Client Implementer's Guide 1.0. See the OpenID Connect Basic Client Implementer's Guide 1.0 (http://openid.net/specs/openid-connect-implicit-1_0.html#OpenID.Basic) for its usage in that subset of OpenID Connect.</summary>
        public const String OfflineAccess = "offline_access";
    }
    
    public static class HttpHeaders
    {
        public const String DPoP = "DPoP";
        public const String DPoPNonce = "DPoP-Nonce";
    }
}