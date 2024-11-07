import {defineConfig} from 'vite'
import path from "path";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.join(__dirname, "./src/"), // path记得引入
    },
  },
  css: {
    modules: {
      localsConvention: "camelCase", // css文件使用 aa-bb,tsx使用aaBb
    },
  },
  server: {
    // port: 3000,
    // open: true,
    proxy: {
      "/api": {
        target: "http://localhost:5162",
        changeOrigin: true,
        // rewrite: (path) => path.replace(/^\/api/, '')
      },
    },
  },
});
