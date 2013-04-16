class Object
  #
  # implements the thrush operator so we
  # can violate the law of dementer.
  # Txy = yx
  #
  def into expr = nil
    expr.nil? ? yield(self) : expr.to_proc.call(self)
  end

  #
  # a thrush that allows you to define a new
  # local scope for an expression.
  #
  def let it
    yield it
  end
end

#
# forwards the left hand side into a method
#
puts (1..10).select(&:odd?).inject(&:+).into { |x| x * x }
