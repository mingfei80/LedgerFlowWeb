# Copilot Instructions

## Project Guidelines
- Backend should no longer depend on Azure AD; keep frontend Microsoft SSO simple and use bearer-token-based auth, with Azure AD code to be removed later.
- Use supported account type 'Any Entra ID tenant + Personal Microsoft account' for the LedgerFlowWeb Microsoft SSO app registration.