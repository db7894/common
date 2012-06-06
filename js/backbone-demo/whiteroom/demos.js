//------------------------------------------------------------
// what we are going to look at (hint: this stuff)
//------------------------------------------------------------
//* jquery
//* underscore.js
//* backbone.js
//* your_candy.js
//* elasticsearch (or any datastore with a http service)
//  - mongodb, couchdb, odata, wcf rest,
//  - json + http service != REST







//------------------------------------------------------------
// basic event system
//------------------------------------------------------------
//$('body').bind('callme', function(e) { alert(e); });
//$('body').trigger('callme');
//$('body').unbind('callme');






//------------------------------------------------------------
// pub/sub system
//------------------------------------------------------------
/*
$('body').bind('callme', function(e,m) { alert(m.message); });
$('body').trigger('callme', { message: 'hello world' });
$('body').unbind('callme');
*/






//------------------------------------------------------------
// model changed listener
//------------------------------------------------------------
/*
function Model() {
  this.props = {
    name: "default",
    age: "default"
  };
  this.bus = $('<div/>');
}

// evented setters and getters
Model.prototype.set = function(name, value) {
  if (this.props[name] !== value) {
    this.props[name] = value;
    this.bus.trigger('changed', { model: this, key: name });
  }
};
Model.prototype.get = function(name) {
  return this.props[name];
};

// let's give it a whirl
var model = new Model();
model.bus.bind('changed', function(e, m) {
  alert(m.key + ' has been change to ' + m.model.get(m.key));
});
model.set('name', 'Superman');
model.set('age', 22);
*/






//------------------------------------------------------------
// model changed using backbone
//------------------------------------------------------------
/*
var QuickModel = Backbone.Model.extend();
var model = new QuickModel();
model.bind('change:name', function(m) {
  alert('previous: ' + m.previous('name'));
  alert('current: '  + m.get('name'));
});
model.set('name', 'Superman');
model.set('age', 22);
model.unbind('change:name');
model.set('name', 'Superperson');
*/






//------------------------------------------------------------
// collections are many models
//------------------------------------------------------------
/*
var QuickModel = Backbone.Model.extend();
var QuickCollection = Backbone.Collection.extend({
  model: QuickModel,

  comparator: function(event) {
    return event.get('number');
  }

});

var collection = new QuickCollection([
  new QuickModel({ name: 'Chuck' }),
]);
collection.each(function(m) {
  $('<p/>')
    .text(m.cid + ' ' + m.get('name'))
    .addClass('common-name')
    .css({ 'font-size': '5em', 'margin': '1em' })
    .appendTo('body');
});
collection.bind('add', function(m) {
  $('<p/>')
    .text(m.cid + ' ' + m.get('name'))
    .addClass('common-name')
    .css({ 'font-size': '5em', 'margin': '1em' })
    .appendTo('body');
});

function reload() {
  if (collection.length < 10) {
    collection.add(new QuickModel({ name: 'Mary' }));
    _.delay(reload, 1000);
  }
}
// reload

collection.bind('reset', function(){
  $('.common-name').remove();
});
//collection.reset();
//collection.unbind('reset');
//collection.unbind('add');
*/






//------------------------------------------------------------
// what about views
//------------------------------------------------------------
/*
$('<p/>').text('something')
  .addClass('well')
  .appendTo('body')
  .wrap('<strong/>');

var layout = _.template("<p class='well'><strong><%= text %></strong></p>");
$('body').append(layout({ text: "something" }));

var layout2 = _.template($('#example-template').html());
$('body').append(layout2({ text: "something" }));
*/






//------------------------------------------------------------
// backbone views
//------------------------------------------------------------
/*
var Simple = Backbone.Model.extend();
var SimpleView = Backbone.View.extend({

  tagName: 'div',
  template: _.template($('#example-template').html()),
  events: {
    'mouseover .event': 'gainfocus',
    'mouseout  .event': 'losefocus'
  },
  gainfocus: function() {
    this.$el.addClass('focused');
  },

  losefocus: function() {
    this.$el.removeClass('focused');
  },

  initialize: function() {
    _.bindAll(this);
    this.model.bind('change',  this.render, this);
  },

  render: function() {
   this.$el
       .empty()
       .html(this.template(this.model.toJSON()));
   return this;
  }
});
var model = new Simple({ text: "Something" });
var view  = new SimpleView({ model: model });

$('body').append(view.render().el);
model.set('text', 'Something Else');
*/






//------------------------------------------------------------
// let's write our stuff for real (intermission)
//------------------------------------------------------------
