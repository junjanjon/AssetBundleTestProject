
def astyle_cs(gitroot, file_name)
	return if file_name !~ /\.cs$/
	file_path = gitroot + file_name
	option_file_path = File.expand_path(File.dirname(__FILE__)) + '/astyleoptions/astyleoption-cs'
	astyle_command = ["astyle", "--options=#{option_file_path}", file_path].join(' ')
	%x(#{astyle_command})
	%x(git add #{file_name})
end


is_inside_work_tree = %x(git rev-parse --is-inside-work-tree)

if !is_inside_work_tree.include? "true"
	exit
end

child_directory = %x(git rev-parse --show-cdup)
gitroot = Dir.pwd + '/' + child_directory.chomp!

diff_file_names = %x(git diff-index --cached --name-status HEAD -- | grep -E '^[AUM]'| cut -c3-)

diff_file_names.split("\n").each do |file_name|
	astyle_cs(gitroot, file_name)
end

