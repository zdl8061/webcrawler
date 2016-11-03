var config=
    {
        "url": "http://www.hao315.tv/video/{0}/",
        "baseUrl": "http://www.hao315.tv",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": "#content",
            "skip": "0",
            "rules": [{
                "name": "@video_title",
                "selector": "h2",
                "skip": "0",
                "filter": "视频：",
                "childLink": ""
            }, {
                "name": "@video_intro",
                "selector": ".video-ny-bk1txt",
                "skip": "0",
                "filter": "点击查看更多",
                "childLink": ""
            }, {
                "name": "@vidoe_code",
                "selector": ".fl div",
                "skip": "0",
                "filter": "",
                "childLink": "",
                "type":"html"
            }]
        },
        "child": {
            "enable": false,
            "skip": "0",
            "multi": true,
            "container": "",
            "rules": [{
                "name": "",
                "selector": "",
                "skip": "0",
                "filter": ""
            }]
        }
    } 