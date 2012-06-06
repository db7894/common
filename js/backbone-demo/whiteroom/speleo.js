// ------------------------------------------------------------
// templates
// ------------------------------------------------------------
var JST = {};
JST['event']  = _.template($('#event-template').html());
JST['status'] = _.template($('#status-template').html());

// ------------------------------------------------------------
// backbone extensions
// ------------------------------------------------------------
Backbone.View.prototype.close = function() {
    if (this.cleanup) { this.cleanup(); }
    this.remove();
    this.unbind();
};

// ------------------------------------------------------------
// models
// ------------------------------------------------------------
var Event = Backbone.Model.extend({
  defaults: {
    date: '12/02/12',
    time: '12:24:05',
  },
  sync: function(x, m, o) { /* this prevents saving */}
});

var Meta = Backbone.Model.extend({
  defaults: {
    time:  0,
    total: 0,
    score: 0,
    valid: false
  },

  update: function(response) {
    this.set({
      time:  response.took,
      total: response.hits.total,
      count: _.size(response.hits.hits),
      score: response.hits.max_score,
      valid: !response.timed_out
    });
  },

  sync: function(x, m, o) {}
});

// ------------------------------------------------------------
// collections
// ------------------------------------------------------------
var EventCollection = Backbone.Collection.extend({
  model: Event,
  url: 'http://localhost:9200/_all/_search',
  meta: new Meta(),

  comparator: function(event) {
    return event.get('score');
  },

  fetch: function(options) {
    options = _.extend({ type:"POST", dataType: "json" }, options);
    options.data = JSON.stringify(options.data);
    return Backbone.Collection.prototype.fetch.call(this, options);
  },

  parse: function(response) {
    return response.hits.hits.map(function(hit) {
      var event = new Event({
        id: hit.id,
        _index: hit._index,
        _type: hit._type,
        _score: hit.score
      });
      return event.set(hit._source);
    });
  }

});

// ------------------------------------------------------------
// views
// ------------------------------------------------------------
var EventView = Backbone.View.extend({

  tagName: 'li',
  template: window.JST['event'],
  events: {
    'click .event-close': 'close',
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
    this.model.bind('change', this.render, this);
    this.model.bind('destroy', this.close, this);
  },

  render: function() {
   this.$el
       .empty()
       .html(this.template(this.model.toJSON()))
       .addClass('well');
   return this;
  },

  cleanup: function() {
    this.model.unbind('change', this.render);
    this.model.unbind('destroy', this.render);
  }
});

var MetaView = Backbone.View.extend({

  tagName: 'div',
  template: window.JST['status'],
  initialize: function() {
    _.bindAll(this);
    this.model.bind('change', this.render, this);
    this.model.bind('destroy', this.close, this);
  },

  render: function() {
   this.$el.empty();
   this.$el.html(this.template(this.model.toJSON()))
   if (this.className) { this.$el.addClass(this.className); }
   return this;
  },

  cleanup: function() {
    this.model.unbind('change', this.render);
    this.model.unbind('destroy', this.render);
  }
});

var EventCollectionView = Backbone.View.extend({

  tagName: 'ul',
  childView: EventView,
  initialize: function() {
    _.bindAll(this);

    this._views = [];
    this.collection.each(this.append);
    this.collection.bind('add', this.append);
    this.collection.bind('remove', this.reject);
    this.collection.bind('reset', this.rebuild);
  },

  append: function(model) {
    var view = new this.childView({ model: model });
    this._views.push(view);
    if (this._rendered) {
      this.$el.append(view.render().el);
    }
  },

  rebuild: function() {
    _.each(this._views, function(view) { view.close(); });
    this.collection.each(this.append);
  },

  reject: function(model) {
    var view = _.find(this._views, function(v) {
      return v.model === model;
    });
    this._views = _.without(this._views, view);
    view.close();
  },

  render: function() {
    var self  = this,
        elems = this.preRender ? this.preRender(this._views) : this._views;

    this.$el.empty();
    if (this.template) {
      this.$el.html(this.template());
    }
    _.each(elems, function(view) {
      self.$el.append(view.render().el);
    });
    this._rendered = true;
    return this;
  },

  cleanup: function() {
    this.collection.unbind('add', this.append);
    this.collection.unbind('remove', this.reject);
    _.each(this._views, function(view) { view.close(); });
  }

});

// ------------------------------------------------------------
// controllers
// ------------------------------------------------------------
var SearchController = Backbone.View.extend({
  el: $('#event-app'),
  inputEl: $('#new-event'),
  events: {
    'keypress #new-event': 'searchEvents',
    'click #new-event-submit': 'searchEventsButton'
  },
  initialize: function() {
    _.bindAll(this);
    this.collection = new EventCollection();
    this.view = new EventCollectionView({ collection: this.collection });
    this.meta = new MetaView({ model: this.collection.meta });
  },

  searchEventsButton: function(e) {
    e.preventDefault();
    var text = this.inputEl.val();
    if (!text) return;
    this.executeSearch(text);
  },

  searchEvents: function(e) {
    var text = this.inputEl.val();
    if (!text || e.keyCode != 13) return;
    this.executeSearch(text);
  },

  executeSearch: function(text) {
    var query = {
      data: {
        query: { 
          'query_string': { query: text }
        }
      }
    };
    var self = this;
    this.inputEl.val('');
    self.view.$el.hide();
    self.meta.$el.hide();
    this.collection.fetch(query).success(function(response) {
      self.collection.meta.update(response);
      if (!self.view._rendered) {
        $('#events').html(self.view.render().el);
        $('#event-status').html(self.meta.render().el);
      }
      self.view.$el.slideDown();
      self.meta.$el.fadeIn(2000);
    });
  },
});

// ------------------------------------------------------------
// page ready
// ------------------------------------------------------------
jQuery(function initialize($) {
  window.app = new SearchController();
});
