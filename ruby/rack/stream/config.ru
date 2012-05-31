# run with: thin starti -R config.ru -p 4321
# test with: curl localhost:4321
#
require 'rack/stream'

class Firehose
  include Rack::Stream::DSL

  stream do
    after_open do
      count = 0
      @timer = EM.add_periodic_timer(1) do
        if count == 3
          close
        else
          chunk "\nNext Chunk"
          count += 1
        end
      end
    end

    before_close do
      @timer.cancel
      chunk "\nfinished\n"
    end

    [200, {'Content-Type' => 'text/plain'}, ['Starting']]
  end

end

app = Rack::Builder.app do
  use Rack::Stream
  run Firehose.new
end

run app
