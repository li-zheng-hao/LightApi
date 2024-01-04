import {fileURLToPath, URL} from 'node:url'
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons'
import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import UnoCSS from 'unocss/vite'
import {viteMockServe} from "vite-plugin-mock";
import path from "path";
import vueJsx from "@vitejs/plugin-vue-jsx";  // 配置vue使用jsx
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'
// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        vue(),
        AutoImport({
            imports: ['vue', 'vue-router', '@vueuse/core'],
            resolvers: [ElementPlusResolver()],
        }),
        Components({
            dirs: ['src/components'],
            resolvers: [ElementPlusResolver()],
        }),
        UnoCSS(),
        viteMockServe({
            mockPath: './src/mock',
            watchFiles: true,
            logger:true,
        }),
        createSvgIconsPlugin({
            // Specify the icon folder to be cached
            iconDirs: [path.resolve(process.cwd(), 'src/assets/icons')],
            // Specify symbolId format
            symbolId: 'icon-[dir]-[name]',
        }),
        vueJsx()
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
                changeOrigin: true,
                // rewrite: (path) => path.replace(/^\/api/, '')
            }
        }
    }
})
