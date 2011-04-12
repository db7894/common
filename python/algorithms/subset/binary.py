from subset.gray import generate_gray

def generate_subsets(items):
    for mask in generate_gray(len(items)):
        yield [item for item,flag in zip(items, mask) if flag]

def generate_cond_subsets(items, condition):
    result = generate_subsets(items)
    return (r for r in result if condition(r))

def generate_size_subsets(items, size):
    return generate_cond_subsets(items,
        lambda s: len(s) == size)

