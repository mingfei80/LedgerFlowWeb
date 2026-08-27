import { StrictMode } from 'react'
import { MsalProvider } from '@azure/msal-react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import { initializeMsal, msalInstance } from './auth/msalConfig'
import './index.css'

async function bootstrap() {
  await initializeMsal()

  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <MsalProvider instance={msalInstance}>
        <App />
      </MsalProvider>
    </StrictMode>,
  )
}

void bootstrap()
