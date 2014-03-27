from bashwork.sample.sampling import largest_difference

def largest_stock_payout(stocks):
    ''' { 'stock-name': [stock-values] } '''
    return { name: largest_difference(vs) for name, vs in stocks.items() }

def robot_battery_capacity(coordinates):
    ''' [(x,y,z)] '''
    return largest_difference((z for _,_,z in coordinates))
