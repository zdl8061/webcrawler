var config=
    {
        "url": "http://wenda.3158.cn/question/{0}/?state=1",
        "baseUrl": "http://wenda.3158.cn",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": ".itm-list",
            "skip": "0",
            "rules": [{
                "name": "@content",
                "selector": "dd a",
                "skip": "0",
                "filter": "",
                "childLink": "href"
            }]
        },
        "child": {
            "enable": true,
            "skip": "0",
            "multi": true,
            "container": ".answ-item",
            "rules": [{
                "name": "@answer",
                "selector": ".txt span",
                "skip": "0",
                "filter": ""
            }]
        }
    }