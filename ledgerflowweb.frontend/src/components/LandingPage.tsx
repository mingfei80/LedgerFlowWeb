import { useMsal } from '@azure/msal-react'
import { loginRequest } from '../auth/msalConfig'

type LandingPageProps = {
  onSignIn: () => Promise<void>
  isSigningIn: boolean
}

export function LandingPage({ onSignIn, isSigningIn }: LandingPageProps) {
  const { accounts } = useMsal()
  const activeAccount = accounts[0]

  return (
    <main className="page landing-page">
      <section className="hero-panel">
        <span className="eyebrow">Microsoft SSO enabled</span>
        <h1>LedgerFlowWeb</h1>
        <p className="hero-copy">
          Sign in with your Microsoft account to open your dashboard and securely call the LedgerFlowWeb API with a bearer token.
        </p>
        <div className="hero-actions">
          <button type="button" className="primary-button" onClick={onSignIn} disabled={isSigningIn}>
            {isSigningIn ? 'Signing in…' : 'Sign in with Microsoft'}
          </button>
        </div>
        <dl className="detail-list">
          <div>
            <dt>Tenant</dt>
            <dd>Any Entra ID tenant + Personal Microsoft account</dd>
          </div>
          <div>
            <dt>Redirect URI</dt>
            <dd>{window.location.origin}</dd>
          </div>
          <div>
            <dt>Scope</dt>
            <dd>{loginRequest.scopes[loginRequest.scopes.length - 1]}</dd>
          </div>
        </dl>
        {activeAccount ? (
          <p className="status-message">
            Cached account detected for <strong>{activeAccount.username}</strong>. Continue sign-in to refresh your session.
          </p>
        ) : null}
      </section>
    </main>
  )
}
