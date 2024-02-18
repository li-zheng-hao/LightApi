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

import unpluginVueComponents from 'unplugin-vue-components/vite';
import { XNaiveUIResolver } from '@skit/x.naive-ui/unplugin';
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
    unpluginVueComponents({
      resolvers: [XNaiveUIResolver()]
    }),
    UnoCSS(),
    viteMockServe({
      mockPath: './src/mock',
      watchFiles: true,
      logger: true,
      localEnabled:false // 本地开发环境是否启用
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
      "@": path.join(__dirname, "./src"), // path记得引入
    }
  },
  server: {
    port: 8080,
    host: '0.0.0.0',
    proxy: {
      '/api': {
        target: 'http://localhost:5162',
        changeOrigin: true
        // rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
})
