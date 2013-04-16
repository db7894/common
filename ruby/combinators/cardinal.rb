class Object
  #
  # cardinal operator used to modify an
  # argument before we apply an argument to it.
  # Cxyz = xzy
  #
  def cardinal(
  end
end

identity = lambda { |f| f }
thrush = cardinal.call(identity)
