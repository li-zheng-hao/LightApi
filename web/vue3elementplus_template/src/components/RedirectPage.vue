<script lang="tsx">
import { onBeforeMount, onMounted } from 'vue'

export default defineComponent({
  name: 'RedirectPage',
  setup() {
    const route = useRoute()
    const router = useRouter()
    onBeforeMount(() => {
      const { params, query } = route
      const { path } = params
      console.log('path', path)

      router.push({
        path: '/' + (Array.isArray(path) ? path.join('/') : path),
        query
      })
    })
    return () => <div />
  }
})
</script>
