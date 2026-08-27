# LedgerFlowWeb Frontend Microsoft SSO Setup

## Azure app registration updates

Update the existing LedgerFlowWeb app registration in Microsoft Entra:

1. Open `Microsoft Entra admin center` > `Applications` > `App registrations`.
2. Open the app with client ID `412586ba-0c3a-4ab9-9062-0c9162c93163`.
3. In `Authentication`, add a `Single-page application` platform.
4. Add the redirect URI `http://localhost:64003`.
5. If you want personal Microsoft accounts, ensure the supported account type is `Any Entra ID tenant + Personal Microsoft account`.
6. In `Manifest`, set `requestedAccessTokenVersion` to `2` if the portal reports that it is invalid.
7. In `Expose an API`, confirm the Application ID URI is `api://412586ba-0c3a-4ab9-9062-0c9162c93163` and the `user_impersonation` scope exists.
8. In `API permissions`, add the delegated permission for `user_impersonation` if it is not already present.

## Local development

1. Start the API at `https://localhost:7202`.
2. Start the frontend with `npm run dev` from `LedgerFlowWeb.Frontend`.
3. Browse to `http://localhost:64003`.
4. Sign in with Microsoft.
5. After sign-in, use the dashboard button to test a bearer-token-backed API call.

## Environment values

The frontend reads these Vite variables:

- `VITE_API_BASE_URL`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_API_SCOPE`
