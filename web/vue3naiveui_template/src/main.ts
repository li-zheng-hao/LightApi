import { createApp } from 'vue'
import { createPinia } from 'pinia'
import 'virtual:uno.css'
import App from './App.vue'
import router from './router'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import 'virtual:svg-icons-register'
import './assets/main.css'
import {setupNaiveDiscreteApi} from "@/plugin/setupNaiveDiscreteApi";
import { setupDirectives } from './directives/directives'

const app = createApp(App)

// 状态管理
app.use(createPinia().use(piniaPluginPersistedstate))

// 路由
app.use(router)

// 自定义指令
setupDirectives(app)

// 挂载 Naive-ui 脱离上下文的 API
setupNaiveDiscreteApi()

app.mount('#app')
