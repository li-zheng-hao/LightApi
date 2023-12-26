import {fileURLToPath, URL} from 'node:url'
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons'
import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import {NaiveUiResolver} from 'unplugin-vue-components/resolvers'
import UnoCSS from 'unocss/vite'
import {viteMockServe} from "vite-plugin-mock";
import path from "path";
import vueJsx from "@vitejs/plugin-vue-jsx";  // 配置vue使用jsx
// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        vue(), AutoImport({
            imports: [
                'vue',
                {
                    'naive-ui': [
                        'useDialog',
                        'useMessage',
                        'useNotification',
                        'useLoadingBar'
                    ]
                }
            ]
        }),
        Components({
            resolvers: [NaiveUiResolver()]
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
    }
})
