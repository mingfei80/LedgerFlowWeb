import { useMemo, useState } from 'react'
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
} from '@azure/msal-react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { loginRequest, msalInstance } from './auth/msalConfig'
import { Dashboard } from './components/Dashboard'
import { LandingPage } from './components/LandingPage'
import { Layout } from './components/Layout'
import { callApi } from './services/apiClient'
import './App.css'

function App() {
  const { accounts } = useMsal()
  const [isSigningIn, setIsSigningIn] = useState(false)
  const [isLoadingProfile, setIsLoadingProfile] = useState(false)
  const [profileData, setProfileData] = useState<string | null>(null)
  const [profileError, setProfileError] = useState<string | null>(null)

  const account = accounts[0]
  const displayName = useMemo(
    () => account?.name ?? account?.username ?? 'LedgerFlowWeb user',
    [account],
  )
  const email = account?.username ?? 'Unknown email'

  const handleSignIn = async () => {
    setIsSigningIn(true)

    try {
      await msalInstance.loginRedirect(loginRequest)
    } finally {
      setIsSigningIn(false)
    }
  }

  const handleSignOut = async () => {
    await msalInstance.logoutRedirect({
      account: msalInstance.getActiveAccount() ?? accounts[0],
    })
  }

  const handleLoadProfile = async () => {
    setIsLoadingProfile(true)
    setProfileError(null)

    try {
      const result = await callApi('/api/users/me')
      setProfileData(JSON.stringify(result, null, 2))
    } catch (error) {
      setProfileData(null)
      setProfileError(error instanceof Error ? error.message : 'Unable to call the API.')
    } finally {
      setIsLoadingProfile(false)
    }
  }

  return (
    <BrowserRouter>
      <Layout
        isAuthenticated={Boolean(account)}
        displayName={displayName}
        email={email}
        onSignOut={handleSignOut}
      >
        <UnauthenticatedTemplate>
          <Routes>
            <Route
              path="*"
              element={<LandingPage onSignIn={handleSignIn} isSigningIn={isSigningIn} />}
            />
          </Routes>
        </UnauthenticatedTemplate>

        <AuthenticatedTemplate>
          <Routes>
            <Route
              path="/"
              element={
                <Dashboard
                  displayName={displayName}
                  email={email}
                  onLoadProfile={handleLoadProfile}
                  isLoadingProfile={isLoadingProfile}
                  profileData={profileData}
                  profileError={profileError}
                />
              }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </AuthenticatedTemplate>
      </Layout>
    </BrowserRouter>
  )
}

export default App
