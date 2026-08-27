import { acquireApiAccessToken, apiBaseUrl } from '../auth/msalConfig'

export async function callApi(path: string, init?: RequestInit) {
  const accessToken = await acquireApiAccessToken()

  if (!accessToken) {
    throw new Error('Microsoft sign-in is required to acquire an API token.')
  }

  const headers = new Headers(init?.headers)
  headers.set('Authorization', `Bearer ${accessToken}`)

  if (!headers.has('Accept')) {
    headers.set('Accept', 'application/json')
  }

  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers,
  })

  if (!response.ok) {
    const errorText = await response.text()
    throw new Error(`API request failed (${response.status}): ${errorText || response.statusText}`)
  }

  const contentType = response.headers.get('content-type')

  if (contentType?.includes('application/json')) {
    return response.json()
  }

  return response.text()
}
