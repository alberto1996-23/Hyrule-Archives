import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Vite handles the local React development server and build pipeline.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
  },
});
