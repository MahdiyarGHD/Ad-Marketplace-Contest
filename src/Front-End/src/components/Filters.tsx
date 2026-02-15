import { memo, useState } from "react";
import { AdFormats, PriceTypes } from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "./Menu";
import TextTransition from "./TextTransition";
import { ChevronDown, ChevronRight } from "lucide-react";
import MainButton from "./MainButton";
import type { Filter, FilterValues, OptionsFilter } from "../stores/useUIStore";
import useUIStore from "../stores/useUIStore";
import { useShallow } from "zustand/shallow";
import { useNavigate } from "react-router-dom";
import useCategoryStore from "../stores/useCategoryStore";

function Filters({ onApply }: { onApply: (filters?: FilterValues) => void }) {
	const appliedFilters = useUIStore(
		useShallow((state) => state.search?.filters),
	);

	const [values, setValues] = useState<FilterValues>(appliedFilters || {});

	const { getCategory } = useCategoryStore();

	const navigate = useNavigate();

	const filters: Filter[] = [
		{
			key: "categoryId",
			title: "Category",
			type: "text",
			onClick: () => navigate("/select-category/filter"),
		},
		{
			key: "subscribers",
			title: "Subscribers",
			type: "range",
			range: [0, 1_000_000],
			minKey: "minSubscribers",
			maxKey: "maxSubscribers",
		},
		{
			key: "minAverageViews",
			title: "Min Average Views",
			type: "number",
			unit: "Views",
			range: [0, 1_000_000],
		},
		{
			key: "maxPrice",
			title: "Max Price",
			type: "number",
			unit: "TON",
			range: [0, 100_000],
		},
		{
			key: "adFormat",
			title: "Ad Format",
			type: "options",
			options: AdFormats,
		},
		{
			key: "priceType",
			title: "Price Type",
			type: "options",
			options: PriceTypes,
		},
	];

	const renderFilterWithOptions = (filter: OptionsFilter) => {
		return (
			<Menu
				custom={({ onClick }) => (
					<div className="Item" onClick={onClick}>
						{/* <div className="icon"></div> */}
						<div className="body">{filter.title}</div>
						<div className="meta">
							<TextTransition
								text={
									filter.options[values?.[filter.key] as string] || "Select"
								}
							/>
							<ChevronDown />
						</div>
					</div>
				)}
			>
				<DropdownMenu className="right">
					{Object.entries(filter.options).map(([key, type]) => (
						<MenuItem
							key={key}
							title={type}
							onClick={() =>
								setValues((prev) => ({ ...prev, [filter.key]: key }))
							}
						/>
					))}
				</DropdownMenu>
			</Menu>
		);
	};

	const renderFilters = (filter: Filter) => {
		switch (filter.type) {
			case "text":
				return (
					<div className="Item" key={filter.title} onClick={filter.onClick}>
						{/* <div className="icon"></div> */}
						<div className="body">
							<div className="title">{filter.title}</div>
						</div>
						<div className="meta">
							{(filter.key === "categoryId" &&
								values?.[filter.key] &&
								getCategory(values?.[filter.key] as string)?.name) ||
								"Select"}
							<ChevronRight />
						</div>
					</div>
				);
			case "number":
				return (
					<div className="Item" key={filter.title}>
						{/* <div className="icon"></div> */}
						<div className="body">
							<div className="title">{filter.title}</div>
						</div>
						<div className="meta">
							<input
								type="number"
								placeholder="0"
								value={(values?.[filter.key] as number) || ""}
								onChange={(e) =>
									setValues((prev) => ({
										...prev,
										[filter.key]: Number(e.target.value),
									}))
								}
							/>
							{filter.unit}
						</div>
					</div>
				);
			case "range":
				return (
					<div className="Item" key={filter.title}>
						{/* <div className="icon"></div> */}
						<div className="body multiline">
							<div className="title">{filter.title}</div>
							<div className="flex">
								<input
									type="number"
									placeholder="Min"
									value={(values?.[filter.minKey] as number) || ""}
									onChange={(e) =>
										setValues((prev) => ({
											...prev,
											[filter.minKey]: Math.max(
												filter.range[0],
												Number(e.target.value),
											),
										}))
									}
								/>
								<input
									type="number"
									placeholder="Max"
									value={(values?.[filter.maxKey] as number) || ""}
									onChange={(e) =>
										setValues((prev) => ({
											...prev,
											[filter.maxKey]: Math.min(
												Number(e.target.value),
												filter.range[1],
											),
										}))
									}
								/>
							</div>
						</div>
					</div>
				);
			case "options":
				return renderFilterWithOptions(filter);
			default:
				return null;
		}
	};

	return (
		<div className="Filters">
			{filters.map((filter) => (
				<div className="Items" key={filter.key}>
					{renderFilters(filter)}
				</div>
			))}
			<div className="TextButton primary" onClick={() => setValues({})}>
				Clear
			</div>
			<MainButton text="Apply" onClick={() => onApply(values)} />
		</div>
	);
}

export default memo(Filters);
