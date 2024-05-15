
const loadedKey=[]

/**
 * 动态加载外部js文件
 * @param url 外部js文件的url
 * @param key 用于标记是否已经加载过的唯一key
 * @returns {Promise<unknown>}
 */
export function load(url,key) {
    if (loadedKey.includes(key)) {
        return Promise.resolve();
    }
    loadedKey.push(key);
    const script = document.createElement('script');
    script.src = url;

    return new Promise((resolve, reject) => {
        script.onload = () => {
            console.log('finish loading lib from ' + url);
            resolve();
        };
        script.onerror = (error) => {
            console.error('Error loading lib from ' + url,  error);
            reject(error);
        };
        document.head.appendChild(script);
    });
}