package org.action;

import org.apache.camel.Exchange;
import org.apache.camel.spi.DataFormat;

import java.io.InputStream;
import java.io.OutputStream;

public class ReverseDataFormat implements DataFormat {

	@Override
	public void marshal(Exchange exchange, Object graph, OutputStream stream) throws Exception {
		byte[] bytes = exchange.getContext()
			.getTypeConverter().mandatoryConvertTo(byte[].class, graph);
		String body = reverseBytes(bytes);
		stream.write(body.getBytes());
	}

	@Override
	public Object unmarshal(Exchange exchange, InputStream stream) throws Exception {
		byte[] bytes = exchange.getContext()
			.getTypeConverter().mandatoryConvertTo(byte[].class, stream);
		return reverseBytes(bytes);
	}

	private static String reverseBytes(byte[] data) {
		StringBuilder sb = new StringBuilder(data.length);
		for (int i = data.length - 1; i >= 0; i--) {
			char ch = (char)data[i];
			sb.append(ch);
		}

		return sb.toString();
	}
}
