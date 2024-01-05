
import {ref} from 'vue'

/**
 * 示例 定义一个Hook 方便逻辑复用
 * @returns 
 */
export function useExample() {
    
    const a =ref(1)
    const b= ref(2)

    function findRoute() {
        return 1
    }

    return {a,b,findRoute}

}
