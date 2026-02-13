import { FilterIcon, SearchIcon } from "lucide-react";
import { memo, useState } from "react";
import Modal from "./Modal";
import Filters from "./Filters";

function Search({ onSearch }: { onSearch: (query: string) => void }) {
	const [showFilters, setShowFilters] = useState(false);

	return (
		<div className="SearchBar">
			<div className="Input">
				<div className="icon">
					<SearchIcon />
				</div>
				<input
					type="text"
					placeholder="Search..."
					onChange={(e) => onSearch(e.target.value)}
				/>
			</div>
			<div className="Filter icon pointer" onClick={() => setShowFilters(true)}>
				<FilterIcon />
			</div>
			<Modal
				open={showFilters}
				onClose={() => setShowFilters(false)}
				title="Filters"
			>
				<Filters />
			</Modal>
		</div>
	);
}

export default memo(Search);
