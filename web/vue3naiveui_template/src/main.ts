

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import 'virtual:uno.css'
import App from './App.vue'
import router from './router'
import 'virtual:svg-icons-register'
import { useTable } from './plugin/vxetable'
import './assets/main.css'
const app = createApp(App)

app.use(createPinia())
app.use(router)

app.use(useTable)
app.mount('#app')
