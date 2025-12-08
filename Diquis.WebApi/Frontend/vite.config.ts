import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import viteTsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
    base: '/', // Changed from '' to '/' to ensure assets load correctly from root
    plugins: [react(), viteTsconfigPaths()],
    server: {
        open: true,
        port: 3000,
        strictPort: true, // Safety: Fail if port 3000 is busy

        // THE FIX: Explicitly tell the browser to ask Port 3000 for updates
        hmr: {
            clientPort: 3000
        },

        proxy: {
            '/api': {
                // FIX: Use ONLY the https URL. Remove the ";http://..." part.
                target: 'https://localhost:7250',
                secure: false, // Allows self-signed certs (localhost)
                changeOrigin: true
            }
        }
    },
    build: {
        outDir: '../wwwroot',
        emptyOutDir: true // Cleans wwwroot before building
    }
});