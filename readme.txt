本程序仅供学习参考，请在24小时后自觉删除~~

如何使用？
1.解压本程序
2.按照通知上的操作登录安全教育平台，并打开安全教育课程页面(推荐使用谷歌Chrome浏览器打开)
3.按F12打开开发者工具，点击Network
4.刷新页面（即有”距离结课还有x天“的那个页面），在Network的Name里面找到listCourse.do?这个名字并点击
5.滚轮移到Form Data区域，点击view source
6.复制view source内的文本，即以”userProjectId=“开头的那一串，完整复制
7.打开Debug文件夹里的SafeEducationWPF.exe
8.粘贴刚才的字符串至软件上，点击完成按钮
9.重新回到浏览器并后退，刷新后如果显示完成，即为成功
注意，如果点击完成后，等待时间过长（超过1分钟），请关闭程序，重新尝试

本程序已开源，Github地址https://github.com/2387744807/SafeEducation
如果你觉得很棒，就给我点个Star吧

致谢
19届不想透露姓名的徐同学 - 提供了抓包信息