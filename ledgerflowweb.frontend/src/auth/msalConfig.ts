import {
  type AccountInfo,
  BrowserCacheLocation,
  InteractionRequiredAuthError,
  LogLevel,
  PublicClientApplication,
  type SilentRequest,
} from '@azure/msal-browser'

const tenantId = import.meta.env.VITE_AZURE_TENANT_ID ?? '60253700-4474-4765-9205-d2e2b21e5539'
const clientId = import.meta.env.VITE_AZURE_CLIENT_ID ?? '412586ba-0c3a-4ab9-9062-0c9162c93163'
const apiScope =
  import.meta.env.VITE_AZURE_API_SCOPE ??
  'api://412586ba-0c3a-4ab9-9062-0c9162c93163/user_impersonation'

export const apiBaseUrl =
  import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7202'

export const loginRequest = {
  scopes: ['openid', 'profile', 'email', apiScope],
}

export const msalInstance = new PublicClientApplication({
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage,
  },
  system: {
    loggerOptions: {
      loggerCallback: () => undefined,
      logLevel: LogLevel.Error,
    },
  },
})

export async function initializeMsal() {
  await msalInstance.initialize()

  const redirectResponse = await msalInstance.handleRedirectPromise()

  if (redirectResponse?.account) {
    msalInstance.setActiveAccount(redirectResponse.account)
    return
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0]

  if (account) {
    msalInstance.setActiveAccount(account)
  }
}

function getTokenRequest(account: AccountInfo): SilentRequest {
  return {
    ...loginRequest,
    account,
  }
}

export async function acquireApiAccessToken() {
  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0]

  if (!account) {
    throw new Error('No Microsoft account is signed in.')
  }

  msalInstance.setActiveAccount(account)

  try {
    const response = await msalInstance.acquireTokenSilent(getTokenRequest(account))
    return response.accessToken
  } catch (error) {
    if (error instanceof InteractionRequiredAuthError) {
      await msalInstance.acquireTokenRedirect(getTokenRequest(account))
      return null
    }

    throw error
  }
}
