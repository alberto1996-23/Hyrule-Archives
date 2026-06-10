import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Vite handles the local React development server and build pipeline.
export default defineConfig({
  plugins: [react()],
  publicDir: '../images',
  server: {
    port: 3000,
  },
  test: {
    exclude: ['tests/e2e/**', 'node_modules/**'],
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/test/setup.js',
  },
});
