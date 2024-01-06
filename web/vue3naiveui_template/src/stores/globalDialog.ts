import {defineStore} from 'pinia'

export const useGlobalDialog = defineStore('counter', {
    state: () => ({
        title: '对话',
        content: '内容',
        onConfirm: () => {
        },
        onCancel: () => {
        },
        isShow: false
    }),
    getters: {},
    actions: {
        show(title: string, content: string, onClick: () => void, onCancel: () => void) {
            this.title = title
            this.content = content
            this.onConfirm = onClick
            this.onCancel = onCancel
            this.isShow = true
        },
        confirm() {
            this.onConfirm()
            this.reset()
        },
        cancel() {
            this.onCancel()
            this.reset()
        },
        reset() {
            this.title = '对话'
            this.content = '内容'
            this.onConfirm = () => {
            }
            this.onCancel = () => {
            }
            this.isShow = false
        }

    }
})
