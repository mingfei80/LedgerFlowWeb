type DashboardProps = {
  displayName: string
  email: string
  onLoadProfile: () => Promise<void>
  isLoadingProfile: boolean
  profileData: string | null
  profileError: string | null
}

const dashboardCards = [
  {
    title: 'Accounts',
    description: 'Connect and review your ledger accounts after sign-in.',
  },
  {
    title: 'Imports',
    description: 'Prepare CSV import flows such as IG transaction uploads.',
  },
  {
    title: 'Transactions',
    description: 'Track balances, holdings, and reconciliation work in one place.',
  },
]

export function Dashboard({
  displayName,
  email,
  onLoadProfile,
  isLoadingProfile,
  profileData,
  profileError,
}: DashboardProps) {
  return (
    <main className="page dashboard-page">
      <section className="dashboard-hero">
        <div>
          <span className="eyebrow">Overview</span>
          <h1>Welcome back, {displayName}</h1>
          <p className="hero-copy">
            You are signed in as <strong>{email}</strong>. This dashboard is ready for the next LedgerFlowWeb features.
          </p>
        </div>
        <button type="button" className="secondary-button" onClick={onLoadProfile} disabled={isLoadingProfile}>
          {isLoadingProfile ? 'Loading API…' : 'Test bearer token call'}
        </button>
      </section>

      <section className="card-grid" aria-label="Dashboard sections">
        {dashboardCards.map((card) => (
          <article className="info-card" key={card.title}>
            <h2>{card.title}</h2>
            <p>{card.description}</p>
          </article>
        ))}
      </section>

      <section className="info-card api-card">
        <h2>API connection</h2>
        <p>
          Use this button to verify the frontend can acquire a Microsoft access token and call the backend with a bearer token.
        </p>
        {profileData ? <pre>{profileData}</pre> : null}
        {profileError ? <p className="error-message">{profileError}</p> : null}
      </section>
    </main>
  )
}
