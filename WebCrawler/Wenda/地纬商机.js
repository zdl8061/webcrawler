var config=
    {
        "url": "http://lingshou.dv37.com/wen/page{0}status2",
        "baseUrl": "http://lingshou.dv37.com",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": "#tl tr",
            "skip": "0",
            "rules": [{
                "name": "@content",
                "selector": "td a",
                "skip": "0",
                "filter": "...",
                "childLink": "href"
            }]
        },
        "child": {
            "enable": true,
            "multi": false,
            "skip": "1",
            "container": ".lefttext",
            "rules": [{
                "name": "@answer",
                "selector": ".padd p",
                "skip": "0",
                "filter": "回答者:"
            }]
        }
    }