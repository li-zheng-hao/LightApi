export interface ElementPlusCustomThemeConfig {
  elColorPrimary: string,
  elColorPrimaryLight3:string,
  elColorPrimaryLight5:string,
  elColorPrimaryLight7:string,
  elColorPrimaryLight8:string,
  elColorPrimaryLight9:string,
  elBorderRadiusBase:string,
  elBoxShadowLight:string
}

export const changeElementPlusTheme=(customConfig:ElementPlusCustomThemeConfig)=>{
    const node = document.documentElement;
    node.style.setProperty('--el-color-primary',customConfig.elColorPrimary)
    node.style.setProperty('--el-color-primary-light-3',customConfig.elColorPrimaryLight3)
    node.style.setProperty('--el-color-primary-light-5',customConfig.elColorPrimaryLight5)
    node.style.setProperty('--el-color-primary-light-7',customConfig.elColorPrimaryLight7)
    node.style.setProperty('--el-color-primary-light-8',customConfig.elColorPrimaryLight8)
    node.style.setProperty('--el-color-primary-light-9',customConfig.elColorPrimaryLight9)
    node.style.setProperty('--el-border-radius-base',customConfig.elBorderRadiusBase)
    node.style.setProperty('--el-box-shadow-light',customConfig.elBoxShadowLight)
    node.style.setProperty('--el-menu-hover-bg-color',customConfig.elColorPrimary)
    node.style.setProperty('--el-menu-bg-color','#001428')
    node.style.setProperty('--el-menu-text-color','#FFFFFF')
    node.style.setProperty('--el-menu-active-color','#FFFFFF')
}

export const  defaultElementPlusCustomThemeConfig= {
  elColorPrimary: '#2080f0',
  elColorPrimaryLight3:'#42A5F5',
  elColorPrimaryLight5:'#64B5F6',
  elColorPrimaryLight7:'#90CAF9',
  elColorPrimaryLight8:'#BBDEFB',
  elColorPrimaryLight9:'#FFFFFF',
  elBorderRadiusBase:'2px' ,
  elBoxShadowLight: '0px 0px 2px rgba(0, 0, 0, 0.12)' 
}
