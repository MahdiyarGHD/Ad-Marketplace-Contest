import { FilterIcon, SearchIcon } from "lucide-react";
import { memo, useEffect, useState } from "react";
import Filters from "./Filters";
import Modal from "./Modal";
import { handleCoolDown, isEmptyValue } from "../utils/common";
import type { FilterValues } from "../stores/useUIStore";
import useUIStore from "../stores/useUIStore";
import { useShallow } from "zustand/shallow";

function Search({
	type,
	onSearch,
}: {
	type: "channels" | "campaigns";
	onSearch: (
		query: string,
		filters?: FilterValues,
		type?: "channels" | "campaigns",
	) => void;
}) {
	const [query, setQuery] = useState("");
	const [showFilters, setShowFilters] = useState(false);

	const filters = useUIStore(useShallow((state) => state.search?.filters));
	const setFilters = useUIStore(
		useShallow((state) => state.search?.setFilters),
	);

	useEffect(() => {
		handleCoolDown(() => {
			if (query.trim() === "" || query.length < 2) {
				onSearch("", filters, type);
				return;
			}

			onSearch(query, filters, type);
		}, 300);
	}, [query, filters]);

	return (
		<div className="SearchBar">
			<div className="Input">
				<div className="icon">
					<SearchIcon />
				</div>
				<input
					type="text"
					placeholder="Search..."
					value={query}
					onChange={(e) => setQuery(e.target.value.trim())}
				/>
			</div>
			<div className="Filter icon pointer" onClick={() => setShowFilters(true)}>
				<FilterIcon />
				{Object.keys(filters || {}).length > 0 && (
					<div className="count">{Object.keys(filters || {}).length}</div>
				)}
			</div>
			<Modal
				open={showFilters}
				onClose={() => setShowFilters(false)}
				title="Filters"
			>
				<Filters
					onApply={(filters) => {
						const activeFilters = Object.fromEntries(
							Object.entries(filters || {}).filter(
								([_, value]) => !isEmptyValue(value),
							),
						);

						setFilters?.(activeFilters || {});
						setShowFilters(false);
					}}
				/>
			</Modal>
		</div>
	);
}

export default memo(Search);
