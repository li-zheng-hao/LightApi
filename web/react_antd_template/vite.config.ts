import { defineConfig, loadEnv } from 'vite';
import path from 'path';
import react from '@vitejs/plugin-react';
import { visualizer } from 'rollup-plugin-visualizer';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());
  return {
    plugins: [react()],
    resolve: {
      alias: {
        '@': path.join(__dirname, './src/'), // path记得引入
      },
    },
    css: {
      modules: {
        localsConvention: 'camelCase', // css文件使用 aa-bb,tsx使用aaBb
      },
    },
    server: {
      // port: 3000,
      // open: true,
      proxy: {
        '/api': {
          target: 'http://localhost:5162',
          changeOrigin: true,
          // rewrite: (path) => path.replace(/^\/api/, '')
        },
      },
    },
    build: {
      sourcemap: env.VITE_ENABLE_SOURCEMAP === 'true',
      rollupOptions: {
        plugins: [
          visualizer({
            open: true, // 直接在浏览器中打开分析报告
            filename: 'stats.html', // 输出文件的名称
            gzipSize: true, // 显示gzip后的大小
            brotliSize: true, // 显示brotli压缩后的大小
          }),
        ],
      },
    },
  };
});
