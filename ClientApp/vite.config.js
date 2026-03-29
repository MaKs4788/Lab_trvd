// vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
    plugins: [react()],
    server: {
        port: 3000,
        proxy: {
            '/api': {
                target: 'http://localhost:5101',
                changeOrigin: true,
                secure: false
            }
        }
    },
    build: {
        outDir: path.resolve(__dirname, '../wwwroot'),
        emptyOutDir: true,
    }
})