module GraphQLGenerator
  # Reformat code that uses curly brace blocks
  class Reformatter
    INDENT_START_CHARS = ["{", "("]
    INDENT_END_CHARS = ["}", ")"]
    DOC_BLOCK_COMMENT = ["///", "//!"];

    def initialize(indent: "\t")
      @indent = indent
    end

    def reformat(code)
      output = ""
      indent_level = 0
      squeeze_newlines = true

      code.lines.each do |line|
        stripped_line = line.strip

        unless DOC_BLOCK_COMMENT.include?(stripped_line.slice(0, 3))
          if INDENT_END_CHARS.include?(stripped_line[0])
            indent_level -= 1 if indent_level - 1 > 0
            # no blank lines immediately preceding end of block
            output.rstrip!
            output << "\n"
          end

          if stripped_line.empty?
            output << "\n" unless squeeze_newlines
            squeeze_newlines = true
          else
            output << @indent * indent_level << line.lstrip
            squeeze_newlines = false
          end

          if INDENT_START_CHARS.include?(stripped_line[-1])
            indent_level += 1
            # no blank lines following start of block
            squeeze_newlines = true
          end
        else
          output << @indent * indent_level << line.lstrip
        end
      end
      output
    end
  end
end
