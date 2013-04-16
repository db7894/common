class Object
  #
  # implements the kestral operator, which is the
  # same thing as returning or tap.
  #
  def inside(value, &block)
    value.instance_eval(&block)
    value
  end
end

#
# example of usage
#
inside([1, 2, 3, 2]) do
  uniq!
  puts self
end
