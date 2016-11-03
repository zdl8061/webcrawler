var config=
    {
        "url": "http://www.txooo.com/wenda/StorageList_9_1_2_{0}.html",
        "baseUrl": "http://www.txooo.com",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": ".stor_list li",
            "skip": "0",
            "rules": [{
                "name": "@content",
                "selector": ".wenti",
                "skip": "0",
                "filter": "...",
                "childLink": "href"
            }]
        },
        "child": {
            "enable": true,
            "skip": "0",
            "multi": true,
            "container": ".reply_list li",
            "rules": [{
                "name": "@answer",
                "selector": ".con p",
                "skip": "0",
                "filter": ""
            }]
        }
    }