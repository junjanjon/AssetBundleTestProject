require 'FileUtils'

is_inside_work_tree = %x(git rev-parse --is-inside-work-tree)

if !is_inside_work_tree.include? "true"
	exit
end

child_directory = %x(git rev-parse --show-cdup)
gitroot = Dir.pwd + '/' + child_directory.chomp!

FileUtils.chmod_R(0755, File.expand_path(File.dirname(__FILE__)) + '/hooks')
FileUtils.cp_r(File.expand_path(File.dirname(__FILE__)) + '/hooks', gitroot + '.git/')
