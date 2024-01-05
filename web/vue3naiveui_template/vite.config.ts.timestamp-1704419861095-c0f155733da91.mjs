// vite.config.ts
import { fileURLToPath, URL } from "node:url";
import { createSvgIconsPlugin } from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/vite-plugin-svg-icons/dist/index.mjs";
import { defineConfig } from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/vite/dist/node/index.js";
import vue from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import AutoImport from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/unplugin-auto-import/dist/vite.js";
import Components from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/unplugin-vue-components/dist/vite.js";
import { NaiveUiResolver } from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/unplugin-vue-components/dist/resolvers.js";
import UnoCSS from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/unocss/dist/vite.mjs";
import { viteMockServe } from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/vite-plugin-mock/dist/index.js";
import path from "path";
import vueJsx from "file:///E:/Code/LightApi/web/vue3naiveui_template/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";
var __vite_injected_original_import_meta_url = "file:///E:/Code/LightApi/web/vue3naiveui_template/vite.config.ts";
var vite_config_default = defineConfig({
  plugins: [
    vue(),
    AutoImport({
      imports: [
        "vue",
        "vue-router",
        {
          "naive-ui": [
            "useDialog",
            "useMessage",
            "useNotification",
            "useLoadingBar"
          ]
        }
      ]
    }),
    Components({
      dirs: ["src/components"],
      resolvers: [NaiveUiResolver()]
    }),
    UnoCSS(),
    viteMockServe({
      mockPath: "./src/mock",
      watchFiles: true,
      logger: true
    }),
    createSvgIconsPlugin({
      // Specify the icon folder to be cached
      iconDirs: [path.resolve(process.cwd(), "src/assets/icons")],
      // Specify symbolId format
      symbolId: "icon-[dir]-[name]"
    }),
    vueJsx()
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", __vite_injected_original_import_meta_url))
    }
  },
  server: {
    port: 8080,
    proxy: {
      "/api": {
        target: "http://localhost:8910",
        changeOrigin: true
        // rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJFOlxcXFxDb2RlXFxcXExpZ2h0QXBpXFxcXHdlYlxcXFx2dWUzbmFpdmV1aV90ZW1wbGF0ZVwiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRTpcXFxcQ29kZVxcXFxMaWdodEFwaVxcXFx3ZWJcXFxcdnVlM25haXZldWlfdGVtcGxhdGVcXFxcdml0ZS5jb25maWcudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0U6L0NvZGUvTGlnaHRBcGkvd2ViL3Z1ZTNuYWl2ZXVpX3RlbXBsYXRlL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHtmaWxlVVJMVG9QYXRoLCBVUkx9IGZyb20gJ25vZGU6dXJsJ1xyXG5pbXBvcnQgeyBjcmVhdGVTdmdJY29uc1BsdWdpbiB9IGZyb20gJ3ZpdGUtcGx1Z2luLXN2Zy1pY29ucydcclxuaW1wb3J0IHtkZWZpbmVDb25maWd9IGZyb20gJ3ZpdGUnXHJcbmltcG9ydCB2dWUgZnJvbSAnQHZpdGVqcy9wbHVnaW4tdnVlJ1xyXG5pbXBvcnQgQXV0b0ltcG9ydCBmcm9tICd1bnBsdWdpbi1hdXRvLWltcG9ydC92aXRlJ1xyXG5pbXBvcnQgQ29tcG9uZW50cyBmcm9tICd1bnBsdWdpbi12dWUtY29tcG9uZW50cy92aXRlJ1xyXG5pbXBvcnQge05haXZlVWlSZXNvbHZlcn0gZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvcmVzb2x2ZXJzJ1xyXG5pbXBvcnQgVW5vQ1NTIGZyb20gJ3Vub2Nzcy92aXRlJ1xyXG5pbXBvcnQge3ZpdGVNb2NrU2VydmV9IGZyb20gXCJ2aXRlLXBsdWdpbi1tb2NrXCI7XHJcbmltcG9ydCBwYXRoIGZyb20gXCJwYXRoXCI7XHJcbmltcG9ydCB2dWVKc3ggZnJvbSBcIkB2aXRlanMvcGx1Z2luLXZ1ZS1qc3hcIjsgIC8vIFx1OTE0RFx1N0Y2RXZ1ZVx1NEY3Rlx1NzUyOGpzeFxyXG4vLyBodHRwczovL3ZpdGVqcy5kZXYvY29uZmlnL1xyXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xyXG4gICAgcGx1Z2luczogW1xyXG4gICAgICAgIHZ1ZSgpLCBBdXRvSW1wb3J0KHtcclxuICAgICAgICAgICAgaW1wb3J0czogW1xyXG4gICAgICAgICAgICAgICAgJ3Z1ZScsXHJcbiAgICAgICAgICAgICAgICAndnVlLXJvdXRlcicsXHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgJ25haXZlLXVpJzogW1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAndXNlRGlhbG9nJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgJ3VzZU1lc3NhZ2UnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAndXNlTm90aWZpY2F0aW9uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgJ3VzZUxvYWRpbmdCYXInXHJcbiAgICAgICAgICAgICAgICAgICAgXVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBdXHJcbiAgICAgICAgfSksXHJcbiAgICAgICAgQ29tcG9uZW50cyh7XHJcbiAgICAgICAgICAgIGRpcnM6IFsnc3JjL2NvbXBvbmVudHMnXSxcclxuICAgICAgICAgICAgcmVzb2x2ZXJzOiBbTmFpdmVVaVJlc29sdmVyKCldXHJcbiAgICAgICAgfSksXHJcbiAgICAgICAgVW5vQ1NTKCksXHJcbiAgICAgICAgdml0ZU1vY2tTZXJ2ZSh7XHJcbiAgICAgICAgICAgIG1vY2tQYXRoOiAnLi9zcmMvbW9jaycsXHJcbiAgICAgICAgICAgIHdhdGNoRmlsZXM6IHRydWUsXHJcbiAgICAgICAgICAgIGxvZ2dlcjp0cnVlLFxyXG4gICAgICAgIH0pLFxyXG4gICAgICAgIGNyZWF0ZVN2Z0ljb25zUGx1Z2luKHtcclxuICAgICAgICAgICAgLy8gU3BlY2lmeSB0aGUgaWNvbiBmb2xkZXIgdG8gYmUgY2FjaGVkXHJcbiAgICAgICAgICAgIGljb25EaXJzOiBbcGF0aC5yZXNvbHZlKHByb2Nlc3MuY3dkKCksICdzcmMvYXNzZXRzL2ljb25zJyldLFxyXG4gICAgICAgICAgICAvLyBTcGVjaWZ5IHN5bWJvbElkIGZvcm1hdFxyXG4gICAgICAgICAgICBzeW1ib2xJZDogJ2ljb24tW2Rpcl0tW25hbWVdJyxcclxuICAgICAgICB9KSxcclxuICAgICAgICB2dWVKc3goKVxyXG4gICAgXSxcclxuICAgIHJlc29sdmU6IHtcclxuICAgICAgICBhbGlhczoge1xyXG4gICAgICAgICAgICAnQCc6IGZpbGVVUkxUb1BhdGgobmV3IFVSTCgnLi9zcmMnLCBpbXBvcnQubWV0YS51cmwpKVxyXG4gICAgICAgIH1cclxuICAgIH0sXHJcbiAgICBzZXJ2ZXI6IHtcclxuICAgICAgICBwb3J0OiA4MDgwLFxyXG4gICAgICAgIHByb3h5OiB7XHJcbiAgICAgICAgICAgICcvYXBpJzoge1xyXG4gICAgICAgICAgICAgICAgdGFyZ2V0OiAnaHR0cDovL2xvY2FsaG9zdDo4OTEwJyxcclxuICAgICAgICAgICAgICAgIGNoYW5nZU9yaWdpbjogdHJ1ZSxcclxuICAgICAgICAgICAgICAgIC8vIHJld3JpdGU6IChwYXRoKSA9PiBwYXRoLnJlcGxhY2UoL15cXC9hcGkvLCAnJylcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSlcclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUF1VCxTQUFRLGVBQWUsV0FBVTtBQUN4VixTQUFTLDRCQUE0QjtBQUNyQyxTQUFRLG9CQUFtQjtBQUMzQixPQUFPLFNBQVM7QUFDaEIsT0FBTyxnQkFBZ0I7QUFDdkIsT0FBTyxnQkFBZ0I7QUFDdkIsU0FBUSx1QkFBc0I7QUFDOUIsT0FBTyxZQUFZO0FBQ25CLFNBQVEscUJBQW9CO0FBQzVCLE9BQU8sVUFBVTtBQUNqQixPQUFPLFlBQVk7QUFWZ0wsSUFBTSwyQ0FBMkM7QUFZcFAsSUFBTyxzQkFBUSxhQUFhO0FBQUEsRUFDeEIsU0FBUztBQUFBLElBQ0wsSUFBSTtBQUFBLElBQUcsV0FBVztBQUFBLE1BQ2QsU0FBUztBQUFBLFFBQ0w7QUFBQSxRQUNBO0FBQUEsUUFDQTtBQUFBLFVBQ0ksWUFBWTtBQUFBLFlBQ1I7QUFBQSxZQUNBO0FBQUEsWUFDQTtBQUFBLFlBQ0E7QUFBQSxVQUNKO0FBQUEsUUFDSjtBQUFBLE1BQ0o7QUFBQSxJQUNKLENBQUM7QUFBQSxJQUNELFdBQVc7QUFBQSxNQUNQLE1BQU0sQ0FBQyxnQkFBZ0I7QUFBQSxNQUN2QixXQUFXLENBQUMsZ0JBQWdCLENBQUM7QUFBQSxJQUNqQyxDQUFDO0FBQUEsSUFDRCxPQUFPO0FBQUEsSUFDUCxjQUFjO0FBQUEsTUFDVixVQUFVO0FBQUEsTUFDVixZQUFZO0FBQUEsTUFDWixRQUFPO0FBQUEsSUFDWCxDQUFDO0FBQUEsSUFDRCxxQkFBcUI7QUFBQTtBQUFBLE1BRWpCLFVBQVUsQ0FBQyxLQUFLLFFBQVEsUUFBUSxJQUFJLEdBQUcsa0JBQWtCLENBQUM7QUFBQTtBQUFBLE1BRTFELFVBQVU7QUFBQSxJQUNkLENBQUM7QUFBQSxJQUNELE9BQU87QUFBQSxFQUNYO0FBQUEsRUFDQSxTQUFTO0FBQUEsSUFDTCxPQUFPO0FBQUEsTUFDSCxLQUFLLGNBQWMsSUFBSSxJQUFJLFNBQVMsd0NBQWUsQ0FBQztBQUFBLElBQ3hEO0FBQUEsRUFDSjtBQUFBLEVBQ0EsUUFBUTtBQUFBLElBQ0osTUFBTTtBQUFBLElBQ04sT0FBTztBQUFBLE1BQ0gsUUFBUTtBQUFBLFFBQ0osUUFBUTtBQUFBLFFBQ1IsY0FBYztBQUFBO0FBQUEsTUFFbEI7QUFBQSxJQUNKO0FBQUEsRUFDSjtBQUNKLENBQUM7IiwKICAibmFtZXMiOiBbXQp9Cg==
