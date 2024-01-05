import { fileURLToPath, URL } from 'node:url'
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { NaiveUiResolver } from 'unplugin-vue-components/resolvers'
import UnoCSS from 'unocss/vite'
import { viteMockServe } from 'vite-plugin-mock'
import path from 'path'
import vueJsx from '@vitejs/plugin-vue-jsx'
import { visualizer } from 'rollup-plugin-visualizer'
// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    AutoImport({
      imports: [
        'vue',
        'vue-router',
        {
          'naive-ui': ['useDialog', 'useMessage', 'useNotification', 'useLoadingBar']
        }
      ]
    }),
    Components({
      dirs: ['src/components'],
      extensions: ['vue'],
      deep: true,
      resolvers: [NaiveUiResolver()]
    }),
    UnoCSS(),
    viteMockServe({
      mockPath: './src/mock',
      watchFiles: true,
      logger: true
    }),
    createSvgIconsPlugin({
      // Specify the icon folder to be cached
      iconDirs: [path.resolve(process.cwd(), 'src/assets/icons')],
      // Specify symbolId format
      symbolId: 'icon-[dir]-[name]'
    }),
    vueJsx(),
    visualizer({
      // 打包完成后自动打开浏览器，显示产物体积报告
      open: true
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 8080,
    proxy: {
      '/api': {
        target: 'http://localhost:8910',
        changeOrigin: true
        // rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
})
