import { type PropsWithChildren } from 'react'

type LayoutProps = PropsWithChildren<{
  isAuthenticated: boolean
  displayName?: string
  email?: string
  onSignOut: () => Promise<void>
}>

export function Layout({ children, isAuthenticated, displayName, email, onSignOut }: LayoutProps) {
  return (
    <div className="app-shell">
      <header className="top-bar">
        <div>
          <p className="brand">LedgerFlowWeb</p>
          <p className="tagline">React + TypeScript + Microsoft SSO</p>
        </div>
        {isAuthenticated ? (
          <div className="account-panel">
            <div>
              <p className="account-name">{displayName}</p>
              <p className="account-email">{email}</p>
            </div>
            <button type="button" className="secondary-button" onClick={onSignOut}>
              Sign out
            </button>
          </div>
        ) : null}
      </header>
      {children}
    </div>
  )
}
