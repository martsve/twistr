var api = {
    
    types: null,
    

    key: function() {
        return $('#key').val();
    },

    init: function() {
        this.SetTypes();
    },

    SetTypes: function() {
        var self = this;
        $.ajax({
            url: "/api/preferences/info",
            type: "GET",
            headers: { "key": self.key() },
            success: function(data) {
                self.types = {};
                for (var i in data)
                {
                    self.types[data[i].id] = data[i];
                }
                self.Rank();
                self.Suggestions();
            }
         });
    },

    Rank: function() {
        var self = this;        
        $.ajax({
            url: "/api/preferences/rank",
            type: "GET",
            headers: { "key": self.key() },
            success: function(data) {
                $('#rank').empty();
                for (var i in data) {                    
                    var item = data[data.length - 1 - i];
                    $('#rank').append("<li><img src='" + self.types[item.type].image +  "' /><span>"+self.types[item.type].name+"</span></li>");
                }
            }
         });
    },

    Suggestions: function() {
        var self = this;
        $.ajax({
            url: "/api/preferences/suggest",
            type: "GET",
            headers: { "key": self.key() },
            success: function(data) {       
                $('#suggestions').empty();
                for (var i in data) {                    
                    var item = data[i];
                    $('#suggestions').append("<li data-object='"+JSON.stringify(item)+"'><button data-pick='1'><img src='" + self.types[item.typeA].image +  "' /><span>" + item.countA + " " + self.types[item.typeA].name + "</span></button> <b>vs.</b> <button data-pick='2'><img src='" + self.types[item.typeB].image +  "' /><span>" + item.countB + " " + self.types[item.typeB].image + "</span></button></li>");
                }
            }
         });
    },

    Post: function(typeA, countA, typeB, countB) {
        var self = this;
        $.ajax({
            url: "/api/preferences/",
            data: JSON.stringify({
                "bestType": typeA,
                "bestCount": countA,
                "worstType": typeB,
                "worstCount": countB
            }),
            type: "POST",
            contentType: "application/json",
            headers: { "key": self.key() },
            success: function(data) {               
                self.Suggestions();
                self.Rank();
            }
         });
    },

    Preferences: function() {
        var self = this;
        $.ajax({
            url: "/api/preferences",
            type: "GET",
            dataType: "text",
            headers: { "key": self.key() },
            success: function(data) {
                $('#respons').html(data);
            }
         });
    }
};
