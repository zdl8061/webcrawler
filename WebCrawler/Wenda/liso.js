var config=
    {
        "url": "http://ask.liso.cn/list_10_a/{0}/",
        "baseUrl": "http://ask.liso.cn",
        "sleep": "2000",
        "pager": true,
        "encoding": "utf-8",
        "main": {
            "container": ".ask_c_l_hd_span1",
            "skip": "0",
            "rules": [{
                "name": "@content",
                "selector": "a",
                "skip": "0",
                "filter": "",
                "childLink": "href"
            }]
        },
        "child": {
            "enable": true,
            "multi": true,
            "container": ".pull_left",
            "skip": "1",
            "rules": [{
                "name": "@answer",
                "selector": ".best_answer_items_infos,.other_mod_infos",
                "skip": "1",
                "filter": ""
            }]
        }
    }