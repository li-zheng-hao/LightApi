import { createApp } from 'vue'
import { createPinia } from 'pinia'
import 'virtual:uno.css'
import App from './App.vue'
import router from './router'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import 'virtual:svg-icons-register'
import './assets/main.css'
import {setupNaiveDiscreteApi} from "@/plugin/setupNaiveDiscreteApi";
const app = createApp(App)



app.use(createPinia().use(piniaPluginPersistedstate))

app.use(router)


setupNaiveDiscreteApi()

app.mount('#app')
