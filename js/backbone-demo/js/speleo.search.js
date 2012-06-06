// ------------------------------------------------------------
// global config
// ------------------------------------------------------------
window.config = {
  eventsPerPage: 3,
  eventPages: 16
};

// ------------------------------------------------------------
// models
// ------------------------------------------------------------
var Facet = Backbone.Model.extend({
  defaults: {
    rank:   0,
    name:  '',
    group: '',
    total:  0
  },

  percent: function() {
    return rank / total;
  },

  sync: function(x, m, o) {}
});

var Event = Backbone.Model.extend({
  defaults: {
    date:  '12/2/2012',
    time:  '12:34:09 PM',
    temp:  Math.floor(Math.random() * 1000000000000),
    text:  '',
    title:  '',
    tags:  ['debug', 'production']
  },

  sync: function(x, m, o) {}
});

var SearchStatus = Backbone.Model.extend({
  defaults: {
    count: 0,
    score: 0.0,
    error: false,
    time:  0,
    pages: [],
    eventsPerPage: window.config.eventsPerPage,
    eventPages: window.config.eventPages
  },

  update: function(data) {
    var pageCount = _.min([ this.get('eventPages'),
        _.size(data.hits.hits) / this.get('eventsPerPage')]);
    this.set({
      'count': data.hits.total,
      'score': data.hits.max_score,
      'error': data.timed_out,
      'time':  data.took,
      'pages': _.range(pageCount)
    });
  }
});

var ChartStatus = Backbone.Model.extend({
  defaults: {
    data: []
  },

  update: function() {
    var groups = Events.groupBy(function(event) {
        //return event.get('date') + event.get('time');
        return event.get('temp');
      }),
      data = _.map(_.keys(groups), function(key) {
        return [parseInt(key), _.size(groups[key])];
      });
    data = data.length > 0 ? data : [[0,0]];
    this.set('data', data, { silent: true });
    this.trigger('change');
  }

});


// ------------------------------------------------------------
// collections
// ------------------------------------------------------------
var EventCollection = Backbone.Collection.extend({
  model: Event,

  comparator: function(event) {
    return event.get('date');
  },

  /**
   * Should I do these filters here or just let
   * elastic search do this?
   */
  withTag:function(tag) {
    return this.filter(function(event) {
      return _.include(event.tags, tag);
    });
  },

  make: function(hit) {
    var source = hit['_source'];
    delete hit['_source'];
    source.tags = [hit['_index'], hit['_type']];
    return new Event(_.extend(source, hit));
  }

});

var FacetCollection = Backbone.Collection.extend({
  model: Facet,

  comparator: function(facet) {
    return facet.get('rank');
  },

  groups: function() {
    return _.keys(this.groupBy(function(facet) {
      return facet.get('group');
    }));
  },

  byGroup:function(group) {
    return this.filter(function(facet) {
      return facet.get('group') === group;
    });
  },

  parse: function(data) {
    var self = this; this.reset();
    _.each(_.keys(data.facets), function(key) {
      _.each(data[key].terms, function(term) {
        self.create({
          'group': key,
          'rank':  term.count,
          'name':  term.term,
          'total': data[key].total
        });
      });
    });
  }

});

// ------------------------------------------------------------
// views
// ------------------------------------------------------------
var EventView = Backbone.View.extend({
  tagName: 'li',
  template: speleo.template['search.event'],

  events: {
    'mouseover .event': 'gainfocus',
    'mouseout  .event': 'losefocus',
    'click .event-close': 'remove'
  },

  initailize: function(args) {
    this.model.bind('change',  this.render, this);
    this.model.bind('destroy', this.remove, this);
  },

  render: function() {
    this.$el.html(this.template(this.model.toJSON()))
      .addClass('well');
    return this;
  },

  remove: function() {
    this.$el.remove();
  },

  gainfocus: function() {
    this.$el.addClass('focused');
  },

  losefocus: function() {
    this.$el.removeClass('focused');
  }
});

var StatusView = Backbone.View.extend({

  el: '#event-status',
  template: speleo.template['search.status'],
  events: {
    'click #event-pager li': 'changePage'
  },

  initialize: function() {
    this.model.bind('change', this.render, this);
  },

  changePage: function(e) {
    var page = e.target.innerHTML;
    $(e.target)
      .parent().addClass('active')
      .siblings().removeClass('active');
    Notifier.trigger('changeEventPage', parseInt(page));
  },

  render: function() {
    this.$el.slideUp();
    this.$el.html(this.template(this.model.toJSON()))
        .slideDown();
    Notifier.trigger('changeEventPage', 0);
    return this;
  }

});


// ------------------------------------------------------------
// main 
// ------------------------------------------------------------
var AppView = Backbone.View.extend({
  /**
   * - client to elastic (search to here and post results)
   * - bind search results to graph (come from service stats)
   * - client to backend
   *   * statistics model (for long term history)
   *   * node modules (for health checking)
   */

  el: $('#speleo-app'),
  listEl: $('#event-list'),
  inputEl: $('#new-event'),
  events: {
    'keypress #new-event': 'searchEvents',
    'click #new-event-submit': 'searchEventsButton'
  },

  initialize: function() {
    Notifier.bind('changeEventPage', this.changeEvents, this);
    _.bindAll(this, 'parseResults', 'addEvent');

    this.elastic = new ElasticSearch({
      callback: this.parseResults
    });
    this.status = new StatusView({ model: new SearchStatus() });
  },

  parseResults: function(data, xhr) {
    this.status.model.update(data);
    Events.reset(_.map(data.hits.hits, Events.make));
  },

  executeSearch: function() {
    var text = this.inputEl.val();
    this.elastic.search({
      queryDSL: {
        query: { 
          'query_string': { query: text }
        }
      }
    });
    this.inputEl.val('');
  },

  searchEventsButton: function(e) {
    e.preventDefault();
    var text = this.inputEl.val();
    if (!text) return;
    this.executeSearch();
  },

  searchEvents: function(e) {
    var text = this.inputEl.val();
    if (!text || e.keyCode != 13) return;
    this.executeSearch();
  },

  addEvent: function(event) {
    var view = new EventView({ model: event }),
        elem = view.render().$el;
    elem.appendTo(this.listEl);
  },

  changeEvents: function(page) {
    var count = window.config.eventsPerPage,
        self  = this;

    this.listEl.fadeOut('fast', function() {
      $(this).html('');
      Events.chain()
        .rest(page * count).first(count)
        .each(self.addEvent)
        .value();
      self.listEl.slideDown();
    });
  }
});


// ------------------------------------------------------------
// page ready
// ------------------------------------------------------------
jQuery(function initialize($) {
  window.Notifier = _.extend({}, Backbone.Events);
  window.Events = new EventCollection();
  window.app = new AppView();
});
