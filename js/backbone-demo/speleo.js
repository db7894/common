// ------------------------------------------------------------
// global
// ------------------------------------------------------------
speleo          = window.speleo   || {};
speleo.model    = speleo.model    || {};
speleo.group    = speleo.group    || {};
speleo.view     = speleo.view     || {};
speleo.option   = speleo.option   || {};
speleo.template = speleo.template || {};
speleo.event    = speleo.event    || _.extend({}, Backbone.Events);

// ------------------------------------------------------------
// templates
// ------------------------------------------------------------
speleo.template["block/simple"] = _.template($('#event-template').html());
speleo.template["block/statistic"] = _.template($('#status-template').html());

// ------------------------------------------------------------
// models
// ------------------------------------------------------------
speleo.model.Query = Backbone.Model.extend({
  defaults: {
    'title': 'New Query',
    'query': '',
    'type':  'block',
    'created': new Date()
  },

  sync: function(x, m, o) {}
});

speleo.model.Meta = Backbone.Model.extend({
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

speleo.model.Event = Backbone.Model.extend({
  defaults: {
    date:  '12/2/2012',
    time:  '12:34:09 PM',
    score: 0
  },

  sync: function(x, m, o) {}
});


// ------------------------------------------------------------
// collections
// ------------------------------------------------------------
speleo.group.Event = Backbone.Collection.extend({
  model: speleo.model.Event,
  url: 'http://localhost:9200/_all/_search',
  meta: new speleo.model.Meta(),

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
      var event = new speleo.model.Event({
        id: hit.id,
        _index: hit._index,
        _type: hit._type,
        _score: hit.score
      });
      return event.set(hit._source);
    });
  }

});

function testing() {
  window.collection = new speleo.group.Event();
  collection.fetch({ data: { query: { match_all: {} } } })
    .success(function(r) {
      collection.meta = new speleo.model.Meta({
        time:  r.took,
        total: r.hits.total,
        count: _.size(r.hits.hits),
        score: r.hits.max_score,
        valid: !r.timed_out
      });
    });
}


// ------------------------------------------------------------
// backbone extensions
// ------------------------------------------------------------
Backbone.View.prototype.close = function() {
    if (this.cleanup) { this.cleanup(); }
    this.remove();
    this.unbind();
};

// ------------------------------------------------------------
// generic views
// ------------------------------------------------------------
speleo.view.GenericModel = Backbone.View.extend({

  tagName: 'div',
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
})

speleo.view.GenericGroup = Backbone.View.extend({

  tagName: 'ul',
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
// specific views
// ------------------------------------------------------------
speleo.view.BlockResult = speleo.view.GenericModel.extend({
  template: speleo.template['block/block'],
  events: {
    'click .block-close': 'close'
  }
});

speleo.view.StatisticResult = speleo.view.GenericModel.extend({
  template: speleo.template['block/statistic'],
  events: {
    'click .block-close': 'close'
  }
});

speleo.view.SimpleResult = speleo.view.GenericModel.extend({
  tagName: 'li',
  className: 'well',
  template: speleo.template['block/simple'],
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
  }
});

speleo.view.TopListResult = speleo.view.GenericGroup.extend({
  childView:speleo.view.SimpleResult,
  preRender: function(views) {
    return _.chain(views).sortBy(function(v) {
      return v.model.score;
    }).first(5).value();
  }
});

speleo.view.PagedListResult = speleo.view.GenericGroup.extend({
  childView:speleo.view.SimpleResult
});

function test_top_result() {
  var collection = new speleo.group.Event();
  collection.fetch({ data: { query: { match_all: {} } } })
    .success(function(response) {
      collection.meta.update(response);
      var view = new speleo.view.PagedListResult({ collection: collection });
      $('#events').html(view.render().el);

      var meta = new speleo.view.StatisticResult({ model: collection.meta });
      $('#event-status').html(meta.render().el);
    });
}

// ------------------------------------------------------------
// controller
// ------------------------------------------------------------
speleo.view.SearchController = Backbone.View.extend({
  el: $('#event-app'),
  inputEl: $('#new-event'),
  events: {
    'keypress #new-event': 'searchEvents',
    'click #new-event-submit': 'searchEventsButton'
  },
  initialize: function() {
    _.bindAll(this);
    this.collection = new speleo.group.Event();
    this.view = new speleo.view.PagedListResult({ collection: this.collection });
    this.meta = new speleo.view.StatisticResult({ model: this.collection.meta });
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
  window.app = new speleo.view.SearchController();
});
