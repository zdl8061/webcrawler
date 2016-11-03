var config=
    {
        "url": "http://wen.28.com/wentiku.php?page={0}",
        "baseUrl": "http://wen.28.com",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": ".active li",
            "skip": "2",
            "rules": [{
                "name": "@content",
                "selector": "a",
                "skip": "0",
                "filter": "...",
                "childLink": "href"
            }, {
                "name": "@class",
                "selector": ".belong",
                "skip": "0",
                "filter": "",
                "childLink": ""
            }]
        },
        "child": {
            "enable": true,
            "skip": "0",
            "multi": true,
            "container": ".answer",
            "rules": [{
                "name": "@answer",
                "selector": ".A-content",
                "skip": "0",
                "filter": ""
            }, {
                "name": "@time",
                "selector": ".A-time",
                "skip": "0",
                "filter": ""
            }]
        }
    }