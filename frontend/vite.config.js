import process from 'node:process'
import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')

  const apiTarget =
    env.services__api__https__0 ||
    env.services__api__http__0 ||
    env.API_URL

  const proxy = apiTarget
    ? {
        '/api': {
          target: apiTarget,
          changeOrigin: true,
          secure: false,
          rewrite: (path) => path.replace(/^\/api/, ''),
        },
      }
    : undefined

  return {
    plugins: [react()],
    server: { proxy },
    preview: { proxy },
  }
})
