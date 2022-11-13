kendo.ui.FilterMenu.prototype.options.messages = $.extend(kendo.ui.FilterMenu.prototype.options.messages, {
        and: "و",
        clear: "مسح الفلترة",
        filter: "فلترة",
        info: "معلومات",
        isFalse: "غير صحيح",
        isTrue: "صحيح",
        or: "أو",
        selectValue: "اختر",
    });
    kendo.ui.FilterMenu.prototype.options.operators = $.extend(kendo.ui.FilterMenu.prototype.options.operators, {
        string: {
            eq: "يساوي",
            neq: "لا يساوي",
            startswith: "يبدأ بـ",
            contains: "يحتوي",
            endswith: "ينتهي بـ"
        },
        //filter menu for "number" type columns
        number: {
            eq: "يساوي",
            neq: "لا يساويe",
            gte: "أكبر أو يساوي",
            gt: "أكبر تماماً",
            lte: "أصغر أو يساوي",
            lt: "أصغر تماماً"
        },
        //filter menu for "date" type columns
        date: {
            eq: "يساوي",
            neq: "لا يساويe",
            gte: "أكبر أو يساوي",
            gt: "أكبر تماماً",
            lte: "أصغر أو يساوي",
            lt: "أصغر تماماً"
        }
    });

